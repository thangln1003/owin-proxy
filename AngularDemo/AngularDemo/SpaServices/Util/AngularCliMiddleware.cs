using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngularDemo.SpaServices.Npm;
using AngularDemo.SpaServices.Proxying;
using AngularDemo.Util;

namespace AngularDemo.SpaServices.Util
{
    internal static class AngularCliMiddleware
    {
        private static TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(5); // This is a development-time only feature, so a very long timeout is fine
        private const string LogCategoryName = "Microsoft.AspNetCore.SpaServices";
        private static readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Attach(ISpaBuilder spaBuilder, string npmScriptName)
        {
            var sourcePath = spaBuilder.Options.SourcePath;
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("Cannot be null or empty", "sourcePath");

            if (string.IsNullOrEmpty(npmScriptName))
                throw new ArgumentException("Cannot be null or empty", nameof(npmScriptName));

            // Start Angular CLI and attach to middleware pipeline
            //ILogger logger = LoggerFinder.GetOrCreateLogger(spaBuilder.ApplicationBuilder, "AngularDemo");
            var logger = Logger;
            var angularCliServerInfoTask = StartAngularCliServerAsync(sourcePath, npmScriptName, logger);

            // Everything we proxy is hardcoded to target http://localhost because:
            // - the requests are always from the local machine (we're not accepting remote
            //   requests that go directly to the Angular CLI middleware server)
            // - given that, there's no reason to use https, and we couldn't even if we
            //   wanted to, because in general the Angular CLI server has no certificate
            var targetUriTask = angularCliServerInfoTask.ContinueWith(
                task => new UriBuilder("http", "localhost", task.Result.Port).Uri);

            spaBuilder.UseProxyToSpaDevelopmentServer(() =>
            {
                // On each request, we create a separate startup task with its own timeout. That way, even if
                // the first request times out, subsequent requests could still work.
                var startupTimeout = spaBuilder.Options.StartupTimeout;
                return targetUriTask.WithTimeout(startupTimeout,
                    "The Angular CLI process did not start listening for requests " +
                    $"within the timeout period of {startupTimeout.TotalSeconds} seconds. " +
                    "Check the log output for error information.");
            });
        }

        private static async Task<AngularCliServerInfo> StartAngularCliServerAsync(string sourcePath, string npmScriptName, NLog.ILogger logger)
        {
            var availablePort = TcpPortFinder.FindAvailablePort();
            logger.Info($"Starting @angular/cli on port {(object) availablePort}...");

            var npmScriptRunner = new NpmScriptRunner(
                sourcePath, npmScriptName, $"--port {availablePort}", null);
            npmScriptRunner.AttachToLogger(logger);

            Match openBrowserLine;
            using (EventedStreamStringReader stdErrReader = new EventedStreamStringReader(npmScriptRunner.StdErr))
            {
                try
                {
                    openBrowserLine = await npmScriptRunner.StdOut.WaitForMatch(
                        new Regex("open your browser on (http\\S+)", RegexOptions.None, RegexMatchTimeout));
                }
                catch (EndOfStreamException ex)
                {
                    throw new InvalidOperationException(
                        $"The NPM script '{npmScriptName}' exited without indicating that the " +
                        $"Angular CLI was listening for requests. The error output was: " +
                        $"{stdErrReader.ReadAsString()}", ex);
                }
            }

            var cliServerUri = new Uri(openBrowserLine.Groups[1].Value);
            var serverInfo = new AngularCliServerInfo
            {
                Port = cliServerUri.Port
            };

            // Even after the Angular CLI claims to be listening for requests, there's a short
            // period where it will give an error if you make a request too quickly
            await WaitForAngularCliServerToAcceptRequests(cliServerUri);

            return serverInfo;
        }

        private static async Task WaitForAngularCliServerToAcceptRequests(Uri cliServerUri)
        {
            // To determine when it's actually ready, try making HEAD requests to '/'. If it
            // produces any HTTP response (even if it's 404) then it's ready. If it rejects the
            // connection then it's not ready. We keep trying forever because this is dev-mode
            // only, and only a single startup attempt will be made, and there's a further level
            // of timeouts enforced on a per-request basis.
            var timeoutMilliseconds = 1000;
            using (var client = new HttpClient())
            {
                while (true)
                {
                    do
                    {
                        int num;
                        do
                        {
                            try
                            {
                                // If we get any HTTP response, the CLI server is ready
                                var httpResponseMessage =
                                    await client.SendAsync(
                                        new HttpRequestMessage(HttpMethod.Head, cliServerUri),
                                        new CancellationTokenSource(timeoutMilliseconds).Token);
                                goto label_12;
                            }
                            catch (Exception ex)
                            {
                                num = 1;
                            }
                        }
                        while (num != 1);
                        await Task.Delay(500);
                    }
                    while (timeoutMilliseconds >= 10000);
                    timeoutMilliseconds += 3000;
                }
            }
            label_12:;
        }

        private class AngularCliServerInfo
        {
            public int Port { get; set; }
        }
    }
}
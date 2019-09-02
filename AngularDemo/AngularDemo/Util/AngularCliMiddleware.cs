using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngularDemo.Npm;
using AngularDemo.Proxying;
using App.Demo.Util;
using Microsoft.Owin.Logging;
using NLog;

namespace AngularDemo.Util
{
    internal static class AngularCliMiddleware
    {
        private static TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(5.0);
        private const string LogCategoryName = "Microsoft.AspNetCore.SpaServices";
        private static readonly NLog.ILogger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Attach(ISpaBuilder spaBuilder, string npmScriptName)
        {
            string sourcePath = spaBuilder.Options.SourcePath;
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("Cannot be null or empty", "sourcePath");
            if (string.IsNullOrEmpty(npmScriptName))
                throw new ArgumentException("Cannot be null or empty", nameof(npmScriptName));
            //ILogger logger = LoggerFinder.GetOrCreateLogger(spaBuilder.ApplicationBuilder, "AngularDemo");
            var logger = Logger;
            Task<Uri> targetUriTask = StartAngularCliServerAsync(sourcePath, npmScriptName, logger)
                .ContinueWith<Uri>((Func<Task<AngularCliMiddleware.AngularCliServerInfo>, Uri>) (task =>
                    new UriBuilder("http", "localhost", task.Result.Port).Uri));

            spaBuilder.UseProxyToSpaDevelopmentServer((Func<Task<Uri>>)(() =>
           {
               TimeSpan startupTimeout = spaBuilder.Options.StartupTimeout;
               return targetUriTask.WithTimeout<Uri>(startupTimeout,
                   "The Angular CLI process did not start listening for requests " +
                   $"within the timeout period of {(object)startupTimeout.TotalSeconds} seconds. " +
                   "Check the log output for error information.");
           }));
        }

        private static async Task<AngularCliServerInfo> StartAngularCliServerAsync(string sourcePath, string npmScriptName, NLog.ILogger logger)
        {
            int availablePort = TcpPortFinder.FindAvailablePort();
            logger.Info($"Starting @angular/cli on port {(object) availablePort}...");
            NpmScriptRunner npmScriptRunner = new NpmScriptRunner(sourcePath, npmScriptName, $"--port {(object) availablePort}", (IDictionary<string, string>)null);
            npmScriptRunner.AttachToLogger(logger);
            Match match;
            using (EventedStreamStringReader stdErrReader = new EventedStreamStringReader(npmScriptRunner.StdErr))
            {
                try
                {
                    match = await npmScriptRunner.StdOut.WaitForMatch(new Regex("open your browser on (http\\S+)",
                        RegexOptions.None, AngularCliMiddleware.RegexMatchTimeout));
                }
                catch (EndOfStreamException ex)
                {
                    throw new InvalidOperationException("The NPM script '" + npmScriptName + "' exited without indicating that the Angular CLI was listening for requests. The error output was: " + stdErrReader.ReadAsString(), (Exception)ex);
                }
            }
            Uri cliServerUri = new Uri(match.Groups[1].Value);
            //Uri cliServerUri = new Uri($"http://localhost:{availablePort}");
            AngularCliServerInfo serverInfo = new AngularCliServerInfo()
            {
                Port = cliServerUri.Port
            };
            await WaitForAngularCliServerToAcceptRequests(cliServerUri);
            return serverInfo;
        }

        private static async Task WaitForAngularCliServerToAcceptRequests(Uri cliServerUri)
        {
            int timeoutMilliseconds = 1000;
            using (HttpClient client = new HttpClient())
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
                                HttpResponseMessage httpResponseMessage =
                                    await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, cliServerUri),
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
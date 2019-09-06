using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AngularDemo.SpaServices.Routing;
using Microsoft.Owin;
using Owin;
using WebSocketMiddleware;

namespace AngularDemo.SpaServices.Proxying
{
    /// <summary>
    /// Extension methods for proxying requests to a local SPA development server during
    /// development. Not for use in production applications.
    /// </summary>
    public static class SpaProxyingExtensions
    {
        /// <summary>
        /// Configures the application to forward incoming requests to a local Single Page
        /// Application (SPA) development server. This is only intended to be used during
        /// development. Do not enable this middleware in production applications.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder"/>.</param>
        /// <param name="baseUri">The target base URI to which requests should be proxied.</param>
        public static void UseProxyToSpaDevelopmentServer(
            this ISpaBuilder spaBuilder,
            string baseUri)
        {
            UseProxyToSpaDevelopmentServer(
                spaBuilder,
                new Uri(baseUri));
        }

        /// <summary>
        /// Configures the application to forward incoming requests to a local Single Page
        /// Application (SPA) development server. This is only intended to be used during
        /// development. Do not enable this middleware in production applications.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder"/>.</param>
        /// <param name="baseUri">The target base URI to which requests should be proxied.</param>
        public static void UseProxyToSpaDevelopmentServer(
            this ISpaBuilder spaBuilder,
            Uri baseUri)
        {
            UseProxyToSpaDevelopmentServer(spaBuilder, () => Task.FromResult(baseUri));
        }

        /// <summary>
        /// Configures the application to forward incoming requests to a local Single Page
        /// Application (SPA) development server. This is only intended to be used during
        /// development. Do not enable this middleware in production applications.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="ISpaBuilder"/>.</param>
        /// <param name="baseUriTaskFactory">A callback that will be invoked on each request to supply a <see cref="Task"/> that resolves with the target base URI to which requests should be proxied.</param>
        public static void UseProxyToSpaDevelopmentServer(this ISpaBuilder spaBuilder,
            Func<Task<Uri>> baseUriTaskFactory)
        {
            var applicationBuilder = spaBuilder.ApplicationBuilder;
            //var applicationStoppingToken = GetStoppingToken(applicationBuilder);

            // Since we might want to proxy WebSockets requests (e.g., by default, AngularCliMiddleware
            // requires it), enable it for the app
            applicationBuilder.UseWebSockets(WebSocketEcho);

            // It's important not to time out the requests, as some of them might be to
            // server-sent event endpoints or similar, where it's expected that the response
            // takes an unlimited time and never actually completes
            var neverTimeOutHttpClient = SpaProxy.CreateHttpClientForProxy(Timeout.InfiniteTimeSpan);

            // Proxy all requests to the SPA development server
            applicationBuilder.Use(async (context, next) =>
            {
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                var isController = routeValues.ContainsKey("controller") &&
                                   !routeValues.Values.Any(x => x.ToString().Contains("."));
                var isAction = routeValues.ContainsKey("action");

                if (context.Request.Path.StartsWithSegments(spaBuilder.Options.DefaultApi) ||
                    (isController && isAction))
                {
                    await next();
                }
                else
                {
                    var didProxyRequest = await SpaProxy.PerformProxyRequest(HttpContext.Current,
                        neverTimeOutHttpClient, baseUriTaskFactory(), context.Request.CallCancelled, proxy404s: true);
                }
            });
        }

        private static async Task WebSocketEcho(IWebSocketContext webSocketContext)
        {
            var buffer = new byte[1024];
            var received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));

            while (!webSocketContext.ClientClosed)
            {
                await webSocketContext.Send(
                    new ArraySegment<byte>(buffer, 0, received.Count),
                    received.MessageType,
                    received.EndOfMessage);

                received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));
            }

            await webSocketContext.Close();
        }

        //private static CancellationToken GetStoppingToken(IAppBuilder appBuilder)
        //{
        //    var applicationLifetime = appBuilder.
        //        .ApplicationServices
        //        .GetService(typeof(IApplicationLifetime));
        //    return ((IAppLifetime)applicationLifetime).ApplicationStopping;
        //}
    }
}
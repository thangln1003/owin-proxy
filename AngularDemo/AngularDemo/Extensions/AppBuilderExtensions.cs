using System;
using Owin;

namespace AngularDemo.Extensions
{
    /// <summary>
    /// Extension methods on the IAppBuilder
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Add web socket middleware to owin pipeline that is built using IAppBuilder.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>The application builder.</returns>
        //public static IAppBuilder UseWebSockets(this IAppBuilder app)
        //{
        //    if (app == null)
        //        throw new ArgumentNullException(nameof(app));
        //    return app.Use(WebSocketMiddleware.UseWebSockets());
        //}

        /// <summary>
        /// Add web socket middleware to owin pipeline that is built using IAppBuilder.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="path">The path to handle websocket requests.</param>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>The application builder.</returns>
        //public static IAppBuilder UseWebSockets(this IAppBuilder appBuilder, WebSocketOptions options)
        //{
        //    appBuilder.Use(WebSockets.UseWebsockets(path, onAccept));
        //    return appBuilder;
        //}
    }
}
using System;
using AngularDemo.Util;

namespace AngularDemo
{
    /// <summary>
    /// Extension methods for enabling Angular CLI middleware support.
    /// </summary>
    public static class AngularCliMiddlewareExtensions
    {
        /// <summary>
        /// Handles requests by passing them through to an instance of the Angular CLI server.
        /// This means you can always serve up-to-date CLI-built resources without having
        /// to run the Angular CLI server manually.
        /// 
        /// This feature should only be used in development. For production deployments, be
        /// sure not to enable the Angular CLI server.
        /// </summary>
        /// <param name="spaBuilder">The <see cref="T:Microsoft.AspNetCore.SpaServices.ISpaBuilder" />.</param>
        /// <param name="npmScript">The name of the script in your package.json file that launches the Angular CLI process.</param>
        public static void UseAngularCliServer(this ISpaBuilder spaBuilder, string npmScript)
        {
            if (spaBuilder == null)
                throw new ArgumentNullException(nameof(spaBuilder));
            if (string.IsNullOrEmpty(spaBuilder.Options.SourcePath))
                throw new InvalidOperationException("To use UseAngularCliServer, you must supply a non-empty value for the SourcePath property of SpaOptions when calling UseSpa.");
            AngularCliMiddleware.Attach(spaBuilder, npmScript);
        }
    }
}
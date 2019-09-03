﻿using System;
using Owin;

namespace AngularDemo.SpaServices
{
    internal class SpaDefaultPageMiddleware
    {
        public static void Attach(ISpaBuilder spaBuilder)
        {
            if (spaBuilder == null)
            {
                throw new ArgumentNullException(nameof(spaBuilder));
            }

            var app = spaBuilder.ApplicationBuilder;
            var options = spaBuilder.Options;

            // Rewrite all requests to the default page
            app.UseHandler((request, response, next) =>
            {
                request.Path = options.DefaultPage.Value;

                return next();
            });

            // Serve it as a static file
            // Developers who need to host more than one SPA with distinct default pages can
            // override the file provider
            //app.UseSpaStaticFilesInternal(
            //    options.DefaultPageStaticFileOptions ?? new StaticFileOptions(),
            //    allowFallbackOnServingWebRootFiles: true);

            //// If the default file didn't get served as a static file (usually because it was not
            //// present on disk), the SPA is definitely not going to work.
            //app.Use((context, next) =>
            //{
            //    var message = "The SPA default page middleware could not return the default page " +
            //        $"'{options.DefaultPage}' because it was not found, and no other middleware " +
            //        "handled the request.\n";

            //    // Try to clarify the common scenario where someone runs an application in
            //    // Production environment without first publishing the whole application
            //    // or at least building the SPA.
            //    var hostEnvironment = (IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment));
            //    if (hostEnvironment != null && hostEnvironment.IsProduction())
            //    {
            //        message += "Your application is running in Production mode, so make sure it has " +
            //            "been published, or that you have built your SPA manually. Alternatively you " +
            //            "may wish to switch to the Development environment.\n";
            //    }

            //    throw new InvalidOperationException(message);
            //});
        }
    }
}
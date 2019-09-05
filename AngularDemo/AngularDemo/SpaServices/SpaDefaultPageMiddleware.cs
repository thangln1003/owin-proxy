﻿using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
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
            app.Use((context, next) =>
            {
                var currentRequest = HttpContext.Current.Request.RequestContext;
                var controllerId = currentRequest.RouteData.Values.Count > 0 ? currentRequest.RouteData.GetRequiredString("controller") : null;

                //IController controller = null;
                //IControllerFactory factory = null;
                //try
                //{
                //    factory = ControllerBuilder.Current.GetControllerFactory();
                //    controller = factory.CreateController(currentRequest, controllerId);
                //    controller?.Execute(currentRequest);
                //}
                //finally
                //{
                //    factory.ReleaseController(controller);
                //}

                if (context.Request.Path.StartsWithSegments(spaBuilder.Options.DefaultApi) || controllerId != null)
                {
                    return next();
                }

                context.Request.Path = options.DefaultPage;

                return next();
            });

            // Serve it as a static file
            // Developers who need to host more than one SPA with distinct default pages can
            // override the file provider
            //app.UseSpaStaticFilesInternal(
            //    options.DefaultPageStaticFileOptions ?? new StaticFileOptions(),
            //    allowFallbackOnServingWebRootFiles: true);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(@".\ClientApp\dist")
            });

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
            //    //var hostEnvironment = (IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment));
            //    //if (hostEnvironment != null && hostEnvironment.IsProduction())
            //    if(!HostingEnvironment.IsDevelopmentEnvironment)
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
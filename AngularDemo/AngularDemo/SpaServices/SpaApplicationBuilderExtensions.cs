﻿
using System;
using Owin;

namespace AngularDemo.SpaServices
{
    /// <summary>
    /// Provides extension methods used for configuring an application to
    /// host a client-side Single Page Application (SPA).
    /// </summary>
    public static class SpaApplicationBuilderExtensions
    {
        /// <summary>
        /// Handles all requests from this point in the middleware chain by returning
        /// the default page for the Single Page Application (SPA).
        /// 
        /// This middleware should be placed late in the chain, so that other middleware
        /// for serving static files, MVC actions, etc., takes precedence.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="configuration">
        /// This callback will be invoked so that additional middleware can be registered within
        /// the context of this SPA.
        /// </param>
        public static void UseSpa(this IAppBuilder app, Action<ISpaBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            //Revisit: Use the options configured in DI (or blank if none was configured). We have to clone it
            // otherwise if you have multiple UseSpa calls, their configurations would interfere with one another.
            //var optionsProvider = app.ApplicationServices.GetService<IOptions<SpaOptions>>();
            //var options = new SpaOptions(optionsProvider.Value);

            var options = new SpaOptions();
            var spaBuilder = new DefaultSpaBuilder(app, options);
            configuration.Invoke(spaBuilder);
            SpaDefaultPageMiddleware.Attach(spaBuilder);
        }
    }
}
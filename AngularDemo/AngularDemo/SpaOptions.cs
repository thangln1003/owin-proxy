using System;
using Microsoft.Owin;

namespace AngularDemo
{
    public class SpaOptions
    {
        private PathString _defaultPage = PathString.FromUriComponent("/index.html");

        /// <summary>
        /// Constructs a new instance of <see cref="T:Microsoft.AspNetCore.SpaServices.SpaOptions" />.
        /// </summary>
        public SpaOptions()
        {
        }

        /// <summary>
        /// Constructs a new instance of <see cref="T:Microsoft.AspNetCore.SpaServices.SpaOptions" />.
        /// </summary>
        /// <param name="copyFromOptions">An instance of <see cref="T:Microsoft.AspNetCore.SpaServices.SpaOptions" /> from which values should be copied.</param>
        internal SpaOptions(SpaOptions copyFromOptions)
        {
            this._defaultPage = copyFromOptions.DefaultPage;
            //this.DefaultPageStaticFileOptions = copyFromOptions.DefaultPageStaticFileOptions;
            this.SourcePath = copyFromOptions.SourcePath;
        }

        /// <summary>
        /// Gets or sets the URL of the default page that hosts your SPA user interface.
        /// The default value is <c>"/index.html"</c>.
        /// </summary>
        public PathString DefaultPage
        {
            get
            {
                return this._defaultPage;
            }
            set
            {
                if (string.IsNullOrEmpty(value.Value))
                    throw new ArgumentException("The value for DefaultPage cannot be null or empty.");
                this._defaultPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Microsoft.AspNetCore.Builder.StaticFileOptions" /> that supplies content
        /// for serving the SPA's default page.
        /// 
        /// If not set, a default file provider will read files from the
        /// <see cref="P:Microsoft.AspNetCore.Hosting.IHostingEnvironment.WebRootPath" />, which by default is
        /// the <c>wwwroot</c> directory.
        /// </summary>
        //public StaticFileOptions DefaultPageStaticFileOptions { get; set; }

        /// <summary>
        /// Gets or sets the path, relative to the application working directory,
        /// of the directory that contains the SPA source files during
        /// development. The directory may not exist in published applications.
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the maximum duration that a request will wait for the SPA
        /// to become ready to serve to the client.
        /// </summary>
        public TimeSpan StartupTimeout { get; set; } = TimeSpan.FromSeconds(60.0);
    }
}
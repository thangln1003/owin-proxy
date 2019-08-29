using App.Demo;
using Owin;

namespace AngularDemo
{
    public interface ISpaBuilder
    {
        /// <summary>
        /// The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> representing the middleware pipeline
        /// in which the SPA is being hosted.
        /// </summary>
        IAppBuilder ApplicationBuilder { get; }

        /// <summary>Describes configuration options for hosting a SPA.</summary>
        SpaOptions Options { get; }
    }
}

using Owin;

namespace AngularDemo.SpaServices
{
    public interface ISpaBuilder
    {
        /// <summary>
        /// The <see cref="T:Owin.IAppBuilder" /> representing the middleware pipeline
        /// in which the SPA is being hosted.
        /// </summary>
        IAppBuilder ApplicationBuilder { get; }

        /// <summary>Describes configuration options for hosting a SPA.</summary>
        SpaOptions Options { get; }
    }
}

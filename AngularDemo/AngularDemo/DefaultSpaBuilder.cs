using System;
using App.Demo;
using Owin;

namespace AngularDemo
{
    internal class DefaultSpaBuilder : ISpaBuilder
    {
        public IAppBuilder ApplicationBuilder { get; }

        public SpaOptions Options { get; }

        public DefaultSpaBuilder(IAppBuilder applicationBuilder, SpaOptions options)
        {
            IAppBuilder applicationBuilder1 = applicationBuilder;
            if (applicationBuilder1 == null)
                throw new ArgumentNullException(nameof(applicationBuilder));
            this.ApplicationBuilder = applicationBuilder1;
            SpaOptions spaOptions = options;
            if (spaOptions == null)
                throw new ArgumentNullException(nameof(options));
            this.Options = spaOptions;
        }
    }
}
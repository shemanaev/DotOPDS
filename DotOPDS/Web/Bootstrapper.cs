using DotOPDS.Utils;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.EmbeddedContent;
using Nancy.EmbeddedContent.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace DotOPDS.Web
{
    class Bootstrapper : DefaultNancyBootstrapper
    {
        private byte[] favicon;

        // FIXME
        /*protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = LoadFavIcon()); }
        }*/

        private byte[] LoadFavIcon()
        {
            using (var resourceStream = Resource.AsStream("favicon.ico"))
            {
                var tempFavicon = new byte[resourceStream.Length];
                resourceStream.Read(tempFavicon, 0, (int)resourceStream.Length);
                return tempFavicon;
            }
        }
#if DEBUG
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = "1" }; }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.EnableRequestTracing = true;
        }
#else
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            DiagnosticsHook.Disable(pipelines);
        }
#endif
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.AddEmbeddedDirectory("/static", Resource.Assembly, "..");
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            //var ns = string.Format("{0}.Views", Resource.Assembly.GetName().Name);
            EmbeddedViewLocationProvider.RootNamespaces.Add(Resource.Assembly, Resource.Assembly.GetName().Name);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder);
            }
        }

        void OnConfigurationBuilder(NancyInternalConfiguration x)
        {
            x.ViewLocationProvider = typeof(EmbeddedViewLocationProvider);
        }
    }
}

using DotOPDS.Middleware;
using DotOPDS.Utils;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;
using System;
using System.Net;
using System.Web.Http;

namespace DotOPDS.Web
{
    class WebServer : IDisposable
    {
        private IDisposable server;

        public WebServer(IPEndPoint endpoint)
        {
            var owinbuilder = new AppBuilder();
            OwinServerFactory.Initialize(owinbuilder.Properties);
            new Startup().Configuration(owinbuilder);
            var builder = ServerBuilder.New()
                .SetEndPoint(endpoint)
                .SetOwinApp(owinbuilder.Build())
                .SetServerHeader("DotOPDS");

            server = builder.Start();
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.Formatters.Remove(config.Formatters.JsonFormatter);
            var xmlFormatter = new AtomXmlMediaTypeFormatter();
            config.Formatters.Insert(0, xmlFormatter);

            config.MapHttpAttributeRoutes();

            if (Settings.Instance.Authentication.Enabled)
                appBuilder.UseBasicAuthentication();
            appBuilder.UseWebApi(config);
        }
    }
}

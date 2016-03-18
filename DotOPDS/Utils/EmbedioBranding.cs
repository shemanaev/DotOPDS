using System;
using Unosquare.Labs.EmbedIO;

namespace DotOPDS.Utils
{
    public class EmbedioBranding : WebModuleBase
    {
        public EmbedioBranding()
        {
            AddHandler(ModuleMap.AnyPath, HttpVerbs.Any, (server, context) =>
            {
                context.Response.AddHeader("X-Powered-By", "DotOPDS");
                return false;
            });
        }

        public override string Name
        {
            get { return "Branding Module"; }
        }
    }

    public static class EmbedioBrandingExtensions
    {
        public static WebServer WithBranding(this WebServer webserver)
        {
            if (webserver == null) throw new ArgumentException("Argument cannot be null.", "webserver");

            webserver.RegisterModule(new EmbedioBranding());
            return webserver;
        }
    }
}

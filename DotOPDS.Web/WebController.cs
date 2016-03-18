using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace DotOPDS.Web
{
    public class WebController : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/static/*")]
        public bool GetStatic(WebServer server, HttpListenerContext context)
        {
            // TODO: add headers
            var name = context.Request.Url.Segments.Last();
            AsStream(name).CopyTo(context.Response.OutputStream);
            return true;
        }

        [WebApiHandler(HttpVerbs.Get, "/*")]
        public bool GetIndex(WebServer server, HttpListenerContext context)
        {
            // TODO: add headers
            // TODO: split path by '.' and serve static files like .css and .js
            AsStream("index.html").CopyTo(context.Response.OutputStream);
            return true;
        }

        private static Stream AsStream(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("{0}.{1}", assembly.GetName().Name, name);
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}

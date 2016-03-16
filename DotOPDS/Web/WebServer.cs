using Nancy.Owin;
using Nowin;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DotOPDS.Web
{
    class WebServer : IDisposable
    {
        private IDisposable server;

        public WebServer(IPEndPoint endpoint)
        {
            var appFunc = NancyMiddleware.UseNancy(new NancyOptions { Bootstrapper = new Bootstrapper() })(env =>
            {
                // Optional - use LibOwin for a typed experience around env dictionary.
                //env["owin.ResponseStatusCode"] = 404;
                //env["owin.ResponseReasonPhrase"] = "Not found";
                return Task.FromResult(0);
            });

            var builder = ServerBuilder.New()
                .SetEndPoint(endpoint)
                .SetOwinApp(appFunc)
                .SetServerHeader("DotOPDS");

            server = builder.Start();
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}

using DotOPDS.Controllers;
using DotOPDS.Utils;
using Serilog;
using System;
using System.Reflection;
using System.Threading;
using Unosquare.Labs.EmbedIO;

namespace DotOPDS.Tasks
{
    class ServeTask : IDisposable
    {
        private WebServer server;

        public int Run()
        {
            server = WebServer
                .Create(Settings.Instance.Listen, new EmbedioLog())
                .EnableCors()
                .WithBranding()
                .WithWebApiController<SearchController>()
                .WithWebApiController<DownloadController>();

            foreach (var module in Settings.Instance.Modules)
            {
                LoadModule(module, server);
            }

            var cts = new CancellationTokenSource();
            var task = server.RunAsync(cts.Token);

            Log.Information("Web server started at {Listen}", Settings.Instance.Listen);

            Program.Exit.WaitOne();
            cts.Cancel();

            return 0;
        }

        public void Dispose()
        {
            server.Dispose();
        }

        private static void LoadModule(string path, WebServer server)
        {
            try
            {
                var assembly = Assembly.LoadFile(path);

                if (assembly == null) return;

                server.LoadApiControllers(assembly).LoadWebSockets(assembly);
            }
            catch (Exception ex)
            {
                server.Log.Error(ex.Message);
                server.Log.Error(ex.StackTrace);
            }
        }
    }
}

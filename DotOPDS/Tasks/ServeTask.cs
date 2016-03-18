using DotOPDS.Web;
using NLog;
using System;
using System.Net;

namespace DotOPDS.Tasks
{
    class ServeTask : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private WebServer server;

        public int Run()
        {
            server = new WebServer(new IPEndPoint(IPAddress.Any, Settings.Instance.Port));

            logger.Info("Web server started at http://localhost:{0}/", Settings.Instance.Port);

            Program.Exit.WaitOne();

            return 0;
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}

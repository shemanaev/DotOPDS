﻿using DotOPDS.Web;
using NLog;
using System;
using System.Net;

namespace DotOPDS.Tasks
{
    class ServeTaskArgs : ITaskArgs
    {
        public int Port { get; set; }
    }

    class ServeTask : ITask
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private WebServer server;

        public void Run(ITaskArgs args_)
        {
            var args = (ServeTaskArgs) args_;
            var ipEndPoint = new IPEndPoint(IPAddress.Any, args.Port);
            server = new WebServer(ipEndPoint);

            logger.Info("Web server started at http://{0}:{1}/", ipEndPoint.Address, ipEndPoint.Port);

            Program.Exit.WaitOne();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                server.Dispose();
            }
        }
    }
}

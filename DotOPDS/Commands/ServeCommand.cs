using AustinHarris.JsonRpc;
using DotOPDS.Utils;
using Serilog;
using System;
using System.Net;
using System.Threading;

namespace DotOPDS.Commands
{
    class ServeCommand : ICommand
    {
        public int Run(SharedOptions options)
        {
            var opts = (ServeOptions)options;
            Settings.Load(opts.Config);

            var endpoint = new IPEndPoint(IPAddress.Loopback, Settings.Instance.Port);
            var server = new Web.WebServer(endpoint);

            Log.Debug("Web server started at {Endpoint}", endpoint);

            


            if (opts.Rpc) RunRpc();
            else RunWait();

            return 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void RunWait()
        {
            Console.WriteLine("Hit Ctrl+C to stop");
            while (true) // FIXME: handle console interrupt
            {
                Thread.Sleep(1000);
            }
        }

        static object[] services = new object[] { new ApiService() };
        private void RunRpc()
        {
            var rpcResultHandler = new AsyncCallback(_ => Console.WriteLine(((JsonRpcStateAsync)_).Result));

            for (string line = Console.ReadLine(); !string.IsNullOrEmpty(line); line = Console.ReadLine())
            {
                var state = new JsonRpcStateAsync(rpcResultHandler, null);
                state.JsonRpc = line;
                JsonRpcProcessor.Process(state);
            }
        }
    }
}

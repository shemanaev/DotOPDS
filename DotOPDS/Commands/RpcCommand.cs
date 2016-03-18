using AustinHarris.JsonRpc;
using CommandLine;
using DotOPDS.Utils;
using Serilog;
using System;
using System.IO;
using System.Text;

namespace DotOPDS.Commands
{
    [Verb("rpc",
        HelpText = "Start RPC server.")]
    class RpcOptions : BaseOptions
    {
    }

    class RpcCommand : ICommand
    {
        private static object[] services = new object[] { new ApiService() };
        private Stream input;

        public int Run(BaseOptions options)
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
            Settings.Load(options.Config);
            input = Console.OpenStandardInput();
            ProcessInput();
            Program.Exit.WaitOne();
            input.Dispose();

            return 0;
        }

        private void ProcessInput()
        {
            if (Program.Exit.WaitOne(1)) return;

            var rpcResultHandler = new AsyncCallback(_ => Console.WriteLine(((JsonRpcStateAsync)_).Result));
            var buf = new byte[4096];

            input.BeginRead(buf, 0, buf.Length, ar =>
            {
                int amtRead = input.EndRead(ar);
                var line = Encoding.UTF8.GetString(buf);
                var state = new JsonRpcStateAsync(rpcResultHandler, null);
                state.JsonRpc = line;
                JsonRpcProcessor.Process(state);
                ProcessInput();
            }, null);
        }
    }
}

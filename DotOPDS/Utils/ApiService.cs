using AustinHarris.JsonRpc;
using DotOPDS.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotOPDS.Utils
{
    class ApiService : JsonRpcService
    {
        private ServeTask serveTask;
        private ImportTask importTask;

        [JsonRpcMethod]
        private bool serve()
        {
            if (serveTask == null)
            {
                Task.Factory.StartNew(() => (serveTask = new ServeTask()).Run());
                return true;
            }
            else
            {
                return false;
            }
        }

        [JsonRpcMethod]
        private void stop()
        {
            // http://bitcoin.stackexchange.com/questions/5810/how-do-i-call-json-rpc-api-using-c
            Program.Exit.Set();
        }

        [JsonRpcMethod]
        private bool import(string library, string input)
        {
            if (importTask == null)
            {
                Task.Factory.StartNew(() =>
                {
                    importTask = new ImportTask();
                    importTask.Run(library, input);
                    importTask = null;
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        [JsonRpcMethod("import.status")]
        private object importStatus()
        {
            if (importTask != null)
            {
                return new
                {
                    Phase = importTask.EntriesProcessed == 0 ? "parsing" : "processing",
                    Entries = new
                    {
                        Processed = importTask.EntriesProcessed,
                        Total = importTask.EntriesTotal,
                    }
                };
            }
            return null;
        }

        [JsonRpcMethod]
        private List<string> ls()
        {
            var res = new List<string>();
            foreach (var lib in Settings.Instance.Libraries)
            {
                res.Add(lib.Value.Path);
            }
            return res;
        }
    }
}

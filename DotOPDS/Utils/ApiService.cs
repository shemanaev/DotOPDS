using AustinHarris.JsonRpc;

namespace DotOPDS.Utils
{
    class ApiService : JsonRpcService
    {
        [JsonRpcMethod]
        private void stop()
        {
            // TODO: stop the server
            // http://bitcoin.stackexchange.com/questions/5810/how-do-i-call-json-rpc-api-using-c
        }

        [JsonRpcMethod]
        private double add(double l, double r)
        {
            // {"jsonrpc": "2.0", "method": "add", "params": [42, 23], "id": 1}
            return l + r;
        }
    }
}

using CommandLine;

namespace DotOPDS.Commands
{
    [Verb("serve",
        HelpText = "Start web server.")]
    class ServeOptions : SharedOptions
    {
        [Option('r', "rpc",
            HelpText = "Start JSON-RPC server.")]
        public bool Rpc { get; set; }
    }
}

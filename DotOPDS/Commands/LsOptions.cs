using CommandLine;

namespace DotOPDS.Commands
{
    [Verb("ls",
        HelpText = "List all libraries.")]
    class LsOptions : SharedOptions
    {
    }
}

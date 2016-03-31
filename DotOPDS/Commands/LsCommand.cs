using CommandLine;
using System;

namespace DotOPDS.Commands
{
    [Verb("ls",
        HelpText = "List all libraries.")]
    class LsOptions : BaseOptions
    {
    }

    class LsCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            Settings.Load(options.Config);

            Console.WriteLine("Libraries available:");
            foreach (var lib in Settings.Instance.Libraries)
            {
                Console.WriteLine("\t{0}", lib.Value);
            }

            return 0;
        }
    }
}

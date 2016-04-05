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
            Console.WriteLine("\tID\t\t\t\t\tLocation");
            foreach (var lib in Settings.Instance.Libraries)
            {
                Console.WriteLine("\t{0}\t{1}", lib.Key, lib.Value.Path);
            }

            return 0;
        }
    }
}

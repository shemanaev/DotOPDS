using CommandLine;
using CommandLine.Text;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;

namespace DotOPDS.Commands
{
    [Verb("mv",
        HelpText = "Move library to another location.")]
    class MvOptions : BaseOptions
    {
        [Value(0, MetaName = "from",
            Required = true,
            HelpText = "Current library location.")]
        public string From { get; set; }

        [Value(1, MetaName = "to",
            Required = true,
            HelpText = "New library location.")]
        public string To { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Move library from 'lib' to '../lib_new'.", new MvOptions
                {
                    From = "lib",
                    To = "../lib_new"
                });
            }
        }
    }

    class MvCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            Settings.Load(options.Config);
            var opts = (MvOptions)options;

            var from = Util.Normalize(opts.From);
            var to = Util.Normalize(opts.To);

            foreach (var lib in Settings.Instance.Libraries)
            {
                if (lib.Value.Path == from)
                {
                    lib.Value.Path = to;
                    Settings.Save();
                    Console.WriteLine("Library moved from {0} to {1}.", from, to);
                    return 0;
                }
            }

            Console.Error.WriteLine("Library {0} not found.", from);

            return 1;
        }
    }
}

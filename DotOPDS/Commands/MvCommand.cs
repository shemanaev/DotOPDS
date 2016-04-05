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
        [Value(0, MetaName = "id",
            Required = true,
            HelpText = "Library ID.")]
        public string Id { get; set; }

        [Value(1, MetaName = "to",
            Required = true,
            HelpText = "New library location.")]
        public string To { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Move library with id '296cff32-eb42-418a-ba1c-3b5115ec128c' to 'lib/new/location'.", new MvOptions
                {
                    Id = "296cff32-eb42-418a-ba1c-3b5115ec128c",
                    To = "lib/new/location"
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

            var id = Guid.Parse(opts.Id);
            var to = Util.Normalize(opts.To);

            if (!Settings.Instance.Libraries.ContainsKey(id))
            {
                Console.Error.WriteLine("Library {0} not found.", id);
                return 1;
            }

            var lib = Settings.Instance.Libraries[id];
            var from = lib.Path;
            lib.Path = to;
            Settings.Save();
            Console.WriteLine("Library moved from {0} to {1}.", from, to);
            return 0;
        }
    }
}

using CommandLine;
using CommandLine.Text;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;

namespace DotOPDS.Commands
{
    [Verb("rm",
    HelpText = "Remove library and books from index.")]
    class RmOptions : BaseOptions
    {
        [Value(0, MetaName = "id",
            Required = true,
            HelpText = "Library ID to delete.")]
        public string Lib { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Remove library with ID '296cff32-eb42-418a-ba1c-3b5115ec128c'.", new RmOptions
                {
                    Lib = "296cff32-eb42-418a-ba1c-3b5115ec128c"
                });
            }
        }
    }

    class RmCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            Settings.Load(options.Config);
            var opts = (RmOptions)options;

            var library = Guid.Parse(opts.Lib);
            if (!Settings.Instance.Libraries.ContainsKey(library))
            {
                Console.Error.WriteLine("Library {0} not found.", library);
                return 1;
            }

            var storage = new LuceneIndexStorage();
            var total = storage.RemoveLibrary(Settings.Instance.DatabaseIndex, library.ToString());

            Settings.Instance.Libraries.Remove(library);
            Settings.Save();
            Console.WriteLine("Library {0} removed ({1} books).", library, total);

            return 0;
        }
    }
}

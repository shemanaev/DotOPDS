using CommandLine;
using CommandLine.Text;
using DotOPDS.Tasks;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DotOPDS.Commands
{
    [Verb("import",
    HelpText = "Displays first lines of a file.")]
    class ImportOptions : BaseOptions
    {
        [Value(0, MetaName = "library path",
            Required = true,
            HelpText = "Base path where books located.")]
        public string Library { get; set; }

        [Value(1, MetaName = "input file",
            Required = true,
            HelpText = "Import contents into internal index.")]
        public string Input { get; set; }

        [Option('r', "covers",
            HelpText = "Covers resolver.")]
        public string Covers { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import inpx file", new ImportOptions
                {
                    Library = "path/to/library/files",
                    Input = "lib1.inpx",
                });
            }
        }
    }

    class ImportCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            var opts = (ImportOptions)options;
            Settings.Load(opts.Config);

            var library = Util.Normalize(opts.Library);
            if (!Directory.Exists(library))
            {
                Console.Error.WriteLine("Library directory {0} not found.", library);
                return 1;
            }

            foreach (var lib in Settings.Instance.Libraries)
            {
                if (library.Equals(lib.Value.Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.Error.WriteLine("Library with path {0} already imported!", library);
                    return 1;
                }
            }

            var watch = Stopwatch.StartNew();
            var status = new ConsoleStatus();
            var task = new ImportTask();
            Task.Factory.StartNew(() =>
            {
                task.Run(library, Util.Normalize(opts.Input), opts.Covers);
            });

            while (task.EntriesProcessed == 0)
            {
                status.Update("Parsing file, elapsed {0}", watch.Elapsed);
            }

            status.Clear();
            Console.WriteLine("Using {0} workers", Environment.ProcessorCount);

            var importStart = watch.Elapsed;
            while (task.EntriesProcessed < task.EntriesTotal)
            {
                status.Update("Processed {0} of {1}, {2} entry/sec, elapsed {3}", task.EntriesProcessed, task.EntriesTotal,
                    Math.Truncate(task.EntriesProcessed / watch.Elapsed.TotalSeconds - importStart.TotalSeconds), watch.Elapsed);
            }

            watch.Stop();
            status.Update("Done in {0}", watch.Elapsed);

            return 0;
        }
    }
}

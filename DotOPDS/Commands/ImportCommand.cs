using CommandLine;
using CommandLine.Text;
using DotOPDS.Tasks;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;

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

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import inpx file", new ImportOptions
                {
                    Library = "path/to/library/files",
                    Input = "path/to/lib1.inpx",
                });
            }
        }
    }

    class ImportCommand : ICommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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

            if (Settings.Instance.Libraries.Any(lib => library.Equals(lib.Value.Path, StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.Error.WriteLine("Library with path {0} already imported!", library);
                return 1;
            }

            var indexFile = Util.Normalize(opts.Input);
            if (!File.Exists(indexFile))
            {
                Console.Error.WriteLine("Index file {0} not found.", indexFile);
                return 1;
            }

            var watch = Stopwatch.StartNew();
            var status = new ConsoleStatus();
            using (var task = new ImportTask())
            {
                task.Start(new ImportTaskArgs
                {
                    Library = library,
                    Input = indexFile
                }, (e) =>
                {
                    Console.WriteLine();
                    Console.Error.WriteLine("Bad input file {0}.", opts.Input);
                    _logger.Fatal(e);
                    Environment.Exit(1);
                });

                while (task.EntriesProcessed == 0)
                {
                    if (Program.Exit.WaitOne(1)) return 1;
                    status.Update("Parsing file, elapsed {0}", watch.Elapsed);
                }

                status.Clear();
                Console.WriteLine("Using {0} workers", Environment.ProcessorCount);

                var importStart = watch.Elapsed;
                while (task.EntriesProcessed < task.EntriesTotal)
                {
                    if (Program.Exit.WaitOne(1)) return 1;
                    status.Update("Processed {0} of {1}, {2} book/sec, elapsed {3}", task.EntriesProcessed, task.EntriesTotal,
                        Math.Truncate(task.EntriesProcessed / (watch.Elapsed.TotalSeconds - importStart.TotalSeconds)), watch.Elapsed);
                }

                watch.Stop();
                status.Update("Done in {0} ({1} books)", watch.Elapsed, task.EntriesProcessed);
            }

            return 0;
        }
    }
}

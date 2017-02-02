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
    HelpText = "Imports a library.")]
    class ImportOptions : BaseOptions
    {
        [Value(0, MetaName = "Import Type",
            Required = true,
            HelpText = "Type of media, inpx or pdf.")]
        public string ImportType { get; set; }

        [Value(1, MetaName = "library path",
            Required = true,
            HelpText = "Base path where books located.")]
        public string Library { get; set; }

        [Value(2, MetaName = "input file",
            Required = true,
            HelpText = "For inpx:Import contents into internal index from .inpx file.")]
        public string Input { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import inpx file", new ImportOptions
                {
                    ImportType = "inpx",
                    Library = "path/to/library/files",
                    Input = "path/to/lib1.inpx",
                });
            }
        }
    }
    /// <summary>
    /// Imports books from an .indx file, using a base Library dir for the actual books
    /// </summary>
    class ImportCommand : ICommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public int Run(BaseOptions options)
        {
            string indexFile = "";
            var opts = (ImportOptions)options;
            Settings.Load(opts.Config);

            var type = opts.ImportType;
            if (type != "inpx" && type != "pdf")
            {
                Console.Error.WriteLine("Type {0} not valid, should be inpx or pdf.", type);
                return 1;
            }
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

            if (type == "inpx")
            { // not needed for pdf scanner
                 indexFile = Util.Normalize(opts.Input);
                if (!File.Exists(indexFile))
                {
                    Console.Error.WriteLine("Index file {0} not found.", indexFile);
                    return 1;
                }
            }

            var watch = Stopwatch.StartNew();
            var status = new ConsoleStatus();
            using (var task = new ImportTask())
            {
                task.Start(new ImportTaskArgs
                {
                    ImportType = type,
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

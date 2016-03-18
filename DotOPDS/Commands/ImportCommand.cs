using CommandLine;
using CommandLine.Text;
using DotOPDS.Tasks;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var watch = Stopwatch.StartNew();
            var status = new ConsoleStatus();
            var task = new ImportTask();
            Task.Factory.StartNew(() =>
            {
                task.Run(opts.Library, opts.Input);
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

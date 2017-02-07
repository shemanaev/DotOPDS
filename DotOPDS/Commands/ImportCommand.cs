using CommandLine;
using DotOPDS.Tasks;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NLog;
using DotOPDS.Plugins;
using System.Linq;

namespace DotOPDS.Commands
{
    [Verb("import",
    HelpText = "Run one of import plugins.")]
    class ImportOptions : BaseOptions
    {
        [Value(0, MetaName = "import plugin name",
            Required = true,
            HelpText = "Import plugin name (as defined in 'help').")]
        public string Plugin { get; set; }

        [Value(1, MetaName = "library path",
            Required = false,
            HelpText = "Path to actual files storage.")]
        public string Library { get; set; }

        [Value(2, MetaName = "import plugin options",
            Required = false,
            HelpText = "Import plugin defined options.")]
        public IEnumerable<string> Arguments { get; set; }
    }

    class ImportCommand : ICommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public int Run(BaseOptions options)
        {
            var opts = (ImportOptions)options;
            Settings.Load(opts.Config);
            PluginProvider.Instance.Initialize();

            string pluginName = opts.Plugin;
            bool showHelp = false;

            if (opts.Plugin.ToLower() == "help")
            {
                if (string.IsNullOrWhiteSpace(opts.Library))
                {
                    Console.WriteLine("Available importers:");
                    PluginProvider.Instance.Importers.ForEach(i =>
                    {
                        Console.WriteLine("  {0}\t\t\t{1}", i.Command, i.Name);
                    });
                    Console.WriteLine();

                    return 0;
                }

                pluginName = opts.Library;
                showHelp = true;
            }

            var plugin = PluginProvider.Instance.GetBookProvider(pluginName);
            if (plugin == null)
            {
                Console.Error.WriteLine("Import plugin '{0}' not found.", opts.Plugin);
                return 1;
            }

            if (showHelp)
            {
                Console.WriteLine(plugin.Help);
                return 0;
            }
                        
            var library = Util.Normalize(opts.Library);
            if (!Directory.Exists(library))
            {
                Console.Error.WriteLine("Library directory {0} not found.", library);
                return 1;
            }

            var watch = Stopwatch.StartNew();
            var status = new ConsoleStatus();
            using (var task = new ImportTask())
            {
                task.Start(new ImportTaskArgs
                {
                    Plugin = plugin,
                    Library = library,
                    Args = opts.Arguments.ToArray()
                }, (e) =>
                {
                    Console.WriteLine();
                    Console.Error.WriteLine("Error executing import plugin '{0}': {1}", opts.Plugin, e.Message);
                    _logger.Fatal(e);
                    Environment.Exit(1);
                });

                while (task.EntriesProcessed == 0)
                {
                    if (Program.Exit.WaitOne(1))
                    {
                        Environment.Exit(1);
                        return 1;
                    }
                    status.Update("Preparing to import, elapsed {0:hh\\:mm\\:ss}", watch.Elapsed);
                }

                status.Clear();
                Console.WriteLine("Using {0} workers", Environment.ProcessorCount);

                var importStart = watch.Elapsed;
                while (!task.Finished)
                {
                    if (Program.Exit.WaitOne(1))
                    {
                        Environment.Exit(1);
                        return 1;
                    }
                    status.Update("Processed {0} of {1}, {2} book/sec, elapsed {3:hh\\:mm\\:ss}", task.EntriesProcessed, task.EntriesTotal,
                        Math.Truncate(task.EntriesProcessed / (watch.Elapsed.TotalSeconds - importStart.TotalSeconds)), watch.Elapsed);
                }

                watch.Stop();
                status.Update("Done in {0:hh\\:mm\\:ss} ({1} added/updated, {2} deleted)", watch.Elapsed, task.EntriesProcessed, task.EntriesDeleted);
            }
            
            return 0;
        }
    }
}

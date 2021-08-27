using CommandLine;
using DotOPDS.Shared;
using DotOPDS.Shared.Plugins;
using Kurukuru;

namespace DotOPDS.Manage.Commands;

[Verb("import", HelpText = "Run one of import plugins.")]
internal class ImportOptions
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

internal class ImportCommand
{
    private readonly LibrariesIndex _libraries;
    private readonly PluginProvider _pluginProvider;
    private readonly LuceneIndexStorage _storage;

    public ImportCommand(
        LibrariesIndex libraries,
        PluginProvider pluginProvider,
        LuceneIndexStorage storage)
    {
        _pluginProvider = pluginProvider;
        _libraries = libraries;
        _storage = storage;
    }

    public async Task<int> Run(ImportOptions opts)
    {
        string pluginName = opts.Plugin;
        bool showHelp = false;

        if (opts.Plugin.ToLower() == "help")
        {
            if (string.IsNullOrWhiteSpace(opts.Library))
            {
                Console.WriteLine("Available importers:");
                _pluginProvider.Providers.ForEach(i =>
                {
                    Console.WriteLine("  {0}\t\t\t{1}", i.Command, i.Name);
                });
                Console.WriteLine();

                return 0;
            }

            pluginName = opts.Library;
            showHelp = true;
        }

        var plugin = _pluginProvider.GetBookProvider(pluginName);
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
                        
        var library = opts.Library;
        if (!Directory.Exists(library))
        {
            Console.Error.WriteLine("Library directory {0} not found.", library);
            return 1;
        }

        var libId = _libraries.GetIdByPath(library);
        if (libId == default)
        {
            libId = Guid.NewGuid();
        }

        var startedAt = DateTime.UtcNow;

        await Spinner.StartAsync("Importing books...", async spinner =>
        {
            var entriesProcessed = 0;
            try
            {
                _storage.Open();
                await foreach (var book in plugin.GetBooksAsync(library, opts.Arguments.ToArray()))
                {
                    book.LibraryId = libId;
                    _storage.Insert(book);
                    entriesProcessed++;
                    if (entriesProcessed % 100 == 0)
                    {
                        spinner.Text = $"Importing books ({entriesProcessed})...";
                    }
                }
                spinner.Succeed($"Imported books ({entriesProcessed})");
            } 
            catch (Exception ex)
            {
                var msg = ex.Message;
#if DEBUG
                msg = ex.StackTrace;
#endif
                spinner.Fail($"Error executing import plugin '{opts.Plugin}': {msg}");
            }
        });

        await Spinner.StartAsync("Cleaning up...", async spinner =>
        {
            var entriesDeleted = await Task.Run(() => _storage.CleanupLibrary(libId.ToString(), startedAt));

            spinner.Succeed($"Deleted orphan books ({entriesDeleted})");
        });

        //_storage.Dispose();
        _libraries.AddLibrary(libId, opts.Library);
        await _libraries.Save();

        //var watch = Stopwatch.StartNew();

        //using (var task = new ImportTask())
        //{
        //    task.Start(new ImportTaskArgs
        //    {
        //        Plugin = plugin,
        //        Library = library,
        //        Args = opts.Arguments.ToArray()
        //    }, (e) =>
        //    {
        //        Console.WriteLine();
        //        Console.Error.WriteLine("Error executing import plugin '{0}': {1}", opts.Plugin, e.Message);
        //        //_logger.Fatal(e);
        //        Environment.Exit(1);
        //    });

        //    while (task.EntriesProcessed == 0)
        //    {
        //        if (Program.Exit.WaitOne(1))
        //        {
        //            Environment.Exit(1);
        //            return 1;
        //        }
        //        status.Update("Preparing to import, elapsed {0:hh\\:mm\\:ss}", watch.Elapsed);
        //    }

        //    status.Clear();
        //    Console.WriteLine("Using {0} workers", Environment.ProcessorCount);

        //    var importStart = watch.Elapsed;
        //    while (!task.Finished)
        //    {
        //        if (Program.Exit.WaitOne(1))
        //        {
        //            Environment.Exit(1);
        //            return 1;
        //        }
        //        status.Update("Processed {0} of {1}, {2} book/sec, elapsed {3:hh\\:mm\\:ss}", task.EntriesProcessed, task.EntriesTotal,
        //            Math.Truncate(task.EntriesProcessed / (watch.Elapsed.TotalSeconds - importStart.TotalSeconds)), watch.Elapsed);
        //    }

        //    watch.Stop();
        //    status.Update("Done in {0:hh\\:mm\\:ss} ({1} added/updated, {2} deleted)", watch.Elapsed, task.EntriesProcessed, task.EntriesDeleted);
        //}

        return 0;
    }
}

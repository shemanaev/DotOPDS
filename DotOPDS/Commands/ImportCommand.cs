using DotOPDS.Importers;
using DotOPDS.Utils;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Commands
{
    class ImportCommand : ICommand
    {
        private bool parserRunning = true;
        private bool importerRunning = true;
        private int entriesTotal;
        private int entriesProcessed;
        private IBookImporter importer = new LuceneImporter();
        private ConcurrentQueue<Book> books = new ConcurrentQueue<Book>();

        public int Run(SharedOptions options)
        {
            var opts = (ImportOptions)options;
            Settings.Load(opts.Config);

            // TODO: check for library path existance and duplication in settings
            var libPath = PathUtil.Normalize(opts.Library);

            //Console.WriteLine("{0}\n{1}\n{2}\n{3}", opts.Library, opts.Input, opts.Config, Settings.Instance.Log.Enabled);

            //return 1;

            // Console.WriteLine("Parsing file: {0}", opts.Input);

            var watch = Stopwatch.StartNew();

            /*
            var luc = new LuceneImporter();
            luc.Open(Settings.Instance.Database, Guid.NewGuid());
            int count;
            var books = luc.Search("\"не время для драконов\"", 200, out count);

            foreach (var book in books)
                Console.WriteLine("ID={0}\tTitle={1}", book.LibId, book.Title);
            Console.WriteLine("Total: {0}", count);
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("Done in {0}", watch.Elapsed);

            return 0;
            */

            var input = PathUtil.Normalize(opts.Input);
            var parser = new InpxParser(input);
            parser.OnNewEntry += Parser_OnNewEntry;
            parser.OnFinished += Parser_OnFinished;
            parser.Parse();

            var status = new ConsoleStatus();
            while (parserRunning)
            {
                status.Update("Parsing file, elapsed {0}", watch.Elapsed);
            }

            var libId = Guid.NewGuid();
            importer.Open(Settings.Instance.Database, libId);

            status.Clear();
            Console.WriteLine("Using {0} workers", Environment.ProcessorCount);
            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            var importStart = watch.Elapsed;
            while (entriesProcessed < entriesTotal)
            {
                status.Update("Processed {0} of {1}, {2} entry/sec, elapsed {3}", entriesProcessed, entriesTotal,
                    Math.Truncate(entriesProcessed / watch.Elapsed.TotalSeconds - importStart.TotalSeconds), watch.Elapsed);
            }

            importerRunning = false;
            watch.Stop();

            Settings.Instance.Libraries.Add(libId, libPath);
            Settings.Save();

            status.Update("Done in {0}", watch.Elapsed);

            return 0;
        }

        public void Dispose()
        {
            importer.Dispose();
        }

        private void Parser_OnFinished(object sender)
        {
            parserRunning = false;
        }

        private void Parser_OnNewEntry(object sender, NewEntryEventArgs e)
        {
            entriesTotal++;
            books.Enqueue(e.Book);
        }

        private void ImportWorker()
        {
            while (importerRunning)
            {
                Book book;
                if (!books.IsEmpty && books.TryDequeue(out book))
                {
                    importer.Insert(book);
                    entriesProcessed++;
                }
                Thread.Sleep(1);
            }
        }
    }
}

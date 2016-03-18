using DotOPDS.Importers;
using DotOPDS.Models;
using DotOPDS.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Tasks
{
    class ImportTask
    {
        private bool running = true;
        private volatile int entriesProcessed;
        private IBookImporter importer = new LuceneImporter();
        private ConcurrentQueue<Book> books = new ConcurrentQueue<Book>();

        public int EntriesTotal { get; private set; }
        public int EntriesProcessed { get { return entriesProcessed; } }

        public int Run(string library, string input)
        {
            // TODO: check for library path existance and duplication in settings
            var libPath = PathUtil.Normalize(library);

            var parser = new InpxParser(PathUtil.Normalize(input));
            parser.OnNewEntry += Parser_OnNewEntry;
            parser.Parse().Wait();

            var libId = Guid.NewGuid();
            importer.Open(Settings.Instance.Database, libId);

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            while (EntriesProcessed < EntriesTotal)
            {
                Thread.Sleep(100);
            }

            running = false;

            Settings.Instance.Libraries.Add(libId, new SettingsLibrary { Path = libPath, Covers = "" });
            Settings.Save();

            importer.Dispose();

            return 0;
        }

        private void Parser_OnNewEntry(object sender, NewEntryEventArgs e)
        {
            EntriesTotal++;
            books.Enqueue(e.Book);
        }

        private void ImportWorker()
        {
            while (running)
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

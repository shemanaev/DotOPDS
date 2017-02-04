using DotOPDS.Models;
using DotOPDS.Utils;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Tasks
{
    class ImportTaskArgs : ITaskArgs
    {
        public string Library { get; set; }
        public string Input { get; set; }
    }

    class ImportTask : ITask
    {
        private bool running = true;
        private int entriesProcessed;
        private Guid libId;
        private LuceneIndexStorage importer = new LuceneIndexStorage();
        private ConcurrentQueue<Book> books = new ConcurrentQueue<Book>();

        public int EntriesTotal { get; private set; }
        public int EntriesProcessed => entriesProcessed;
        public int EntriesDeleted { get; private set; }
        public bool Finished { get; private set; }

        public void Run(ITaskArgs args_)
        {
            var args = (ImportTaskArgs)args_;

            var id = Settings.Instance.Libraries.FirstOrDefault(lib =>
                    args.Library.Equals(lib.Value.Path, StringComparison.InvariantCultureIgnoreCase)).Key;

            if (id == Guid.Empty)
            {
                libId = Guid.NewGuid();
            }
            else
            {
                libId = id;
            }

            var parser = new InpxParser(args.Input);
            parser.OnNewEntry += Parser_OnNewEntry;
            parser.Parse().Wait();

            importer.Open(Settings.Instance.DatabaseIndex);

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            while (EntriesProcessed < EntriesTotal)
            {
                Thread.Sleep(100);
            }

            running = false;

            EntriesDeleted = importer.CleanupLibrary(libId.ToString());

            Settings.Instance.Libraries[libId] = new SettingsLibrary { Path = args.Library };
            Settings.Save();

            Finished = true;
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
                    book.LibraryId = libId;
                    importer.Insert(book);
                    Interlocked.Increment(ref entriesProcessed);
                }
                Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                importer.Dispose();
            }
        }
    }
}

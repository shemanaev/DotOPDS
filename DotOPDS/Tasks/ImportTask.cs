using DotOPDS.Models;
using DotOPDS.Plugins;
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
        public IBookProvider Plugin { get; set; }
        public string Library { get; set; }
        public string[] Args { get; set; }
    }

    class ImportTask : ITask
    {
        private bool running = true;
        private bool finished = false;
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

            var startedAt = DateTime.UtcNow;

            args.Plugin.Run(args.Library, args.Args, OnImportBook, OnImportFinished);

            importer.Open(Settings.Instance.DatabaseIndex);

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            while (EntriesProcessed < EntriesTotal || !finished)
            {
                Thread.Sleep(100);
            }

            running = false;

            EntriesDeleted = importer.CleanupLibrary(libId.ToString(), startedAt);

            Settings.Instance.Libraries[libId] = new SettingsLibrary { Path = args.Library };
            Settings.Save();

            Finished = true;
        }

        private void OnImportBook(Book book)
        {
            EntriesTotal++;
            books.Enqueue(book);
        }

        private void OnImportFinished()
        {
            finished = true;
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

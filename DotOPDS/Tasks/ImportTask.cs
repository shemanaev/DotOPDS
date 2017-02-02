using DotOPDS.Importers;
using DotOPDS.Models;
using DotOPDS.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Tasks
{
    /// <summary>
    /// Path to book directory, input file and parser type
    /// </summary>
    class ImportTaskArgs : ITaskArgs
    {

        public string ImportType { get; set; }
        public string Library { get; set; }
        public string Input { get; set; }
    }

    class ImportTask : ITask
    {
        private bool running = true;
        private int entriesProcessed;
        private Guid libId = Guid.NewGuid();
        private IBookImporter importer = new LuceneImporter();
        private ConcurrentQueue<Book> books = new ConcurrentQueue<Book>();

        public int EntriesTotal { get; private set; }
        public int EntriesProcessed => entriesProcessed;

        public void Run(ITaskArgs args_)
        {
            var args = (ImportTaskArgs)args_;
            switch (args.ImportType)
            {
                case "inpx":
                  InpxParser  inpxparser = new InpxParser(args.Input);
                    inpxparser.OnNewEntry += Parser_OnNewEntry;
                    inpxparser.Parse().Wait();
                    break;
                case "pdf":
                   PdfParser pdfparser = new PdfParser(args.Library);
                    pdfparser.OnNewEntry += Parser_OnNewEntry;
                    pdfparser.Parse().Wait();
                    break;
                default:
                    break;
            }



            

            importer.Open(Util.Normalize(Settings.Instance.Database));

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            while (EntriesProcessed < EntriesTotal)
            {
                Thread.Sleep(100);
            }

            running = false;

            // Done adding items so remember this Library in Settings
            Settings.Instance.Libraries.Add(libId, new SettingsLibrary { Path = args.Library });
            Settings.Save();
        }

        private void Parser_OnNewEntry(object sender, NewEntryEventArgs e)
        {
            EntriesTotal++;
            books.Enqueue(e.Book);
        }

        /// <summary>
        /// Place book into the appropriate library
        /// </summary>
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

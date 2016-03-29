﻿using DotOPDS.Importers;
using DotOPDS.Models;
using DotOPDS.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Tasks
{
    class ImportTaskArgs : ITaskArgs
    {
        public string Library { get; set; }
        public string Input { get; set; }
        public string Covers { get; set; }
    }

    class ImportTask : ITask
    {
        private bool running = true;
        private volatile int entriesProcessed;
        private IBookImporter importer = new LuceneImporter();
        private ConcurrentQueue<Book> books = new ConcurrentQueue<Book>();

        public int EntriesTotal { get; private set; }
        public int EntriesProcessed { get { return entriesProcessed; } }

        public void Run(ITaskArgs args_)
        {
            var args = (ImportTaskArgs)args_;
            var parser = new InpxParser(args.Input);
            parser.OnNewEntry += Parser_OnNewEntry;
            parser.Parse().Wait();

            var libId = Guid.NewGuid();
            importer.Open(Util.Normalize(Settings.Instance.Database), libId);

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Factory.StartNew(ImportWorker);
            }

            while (EntriesProcessed < EntriesTotal)
            {
                Thread.Sleep(100);
            }

            running = false;

            Settings.Instance.Libraries.Add(libId, new SettingsLibrary { Path = args.Library, Covers = args.Covers });
            Settings.Save();
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

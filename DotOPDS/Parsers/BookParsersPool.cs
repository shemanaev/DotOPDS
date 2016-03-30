using DotOPDS.Importers;
using DotOPDS.Models;
using System;
using System.Collections.Generic;

namespace DotOPDS.Parsers
{
    class BookParsersPool
    {
        private static BookParsersPool instance = new BookParsersPool();
        public static BookParsersPool Instance { get { return instance; } }

        private Dictionary<string, IBookParser> parsers = new Dictionary<string, IBookParser>();
        private volatile LuceneImporter importer;

        private BookParsersPool()
        {
            parsers.Add("fb2", new Fb2Parser());
        }

        public void Update(Book book)
        {
            if (book.Cover.Has != null) return;
            IBookParser parser = null;

            if (parsers.ContainsKey(book.Ext))
                parser = parsers[book.Ext];

            if (parser == null) return;

            try
            {
                parser.Update(book);
            }
            catch (Exception)
            {
            }

            if (importer == null)
            {
                importer = new LuceneImporter();
                importer.Open(Settings.Instance.Database);
            }
            importer.Replace(book);
        }
    }
}
using DotOPDS.Models;
using DotOPDS.Utils;
using NLog;
using System;
using System.Collections.Generic;

namespace DotOPDS.Parsers
{
    class BookParsersPool
    {
        private static BookParsersPool instance;
        public static BookParsersPool Instance => instance ?? (instance = new BookParsersPool());

        private Dictionary<string, IBookParser> parsers = new Dictionary<string, IBookParser>();
        private volatile LuceneIndexStorage importer;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private BookParsersPool()
        {
            parsers.Add("fb2", new Fb2Parser());
        }

        public void Update(Book book)
        {
            if (book.Cover.Has != null) return;
            logger.Debug("Book being updated, id:{0}", book.Id);

            IBookParser parser = null;

            if (parsers.ContainsKey(book.Ext))
                parser = parsers[book.Ext];

            if (parser == null) return;

            try
            {
                parser.Update(book);
                logger.Debug("Book updated successfully, id:{0}", book.Id);
            }
            catch (Exception)
            {
                logger.Debug("Book update failed, id:{0}", book.Id);
            }

            if (importer == null)
            {
                importer = new LuceneIndexStorage();
                importer.Open(Util.Normalize(Settings.Instance.Database));
            }
            importer.Replace(book);
        }
    }
}
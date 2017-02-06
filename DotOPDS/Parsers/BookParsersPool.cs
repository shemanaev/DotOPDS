using DotOPDS.Models;
using DotOPDS.Plugins;
using DotOPDS.Utils;
using NLog;
using System;

namespace DotOPDS.Parsers
{
    class BookParsersPool
    {
        private static BookParsersPool instance;
        public static BookParsersPool Instance => instance ?? (instance = new BookParsersPool());

        private volatile LuceneIndexStorage importer;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private BookParsersPool()
        {
        }

        public void Update(Book book)
        {
            if (book.UpdatedFromFile) return;
            logger.Debug("Book being updated, id:{0}", book.Id);

            var parser = PluginProvider.Instance.GetFileFormatReader(book.Ext);

            if (parser == null) return;

            bool replace = false;
            try
            {
                using (var stream = FileUtils.GetBookFile(book))
                {
                    replace = parser.Read(book, stream);
                }
                logger.Debug("Book updated successfully, id:{0}", book.Id);
            }
            catch (Exception e)
            {
                logger.Debug("Book update failed, id:{0}. {1}", book.Id, e.Message);
            }

            if (importer == null)
            {
                importer = new LuceneIndexStorage();
                importer.Open(Settings.Instance.DatabaseIndex);
            }
            if (replace)
            {
                book.UpdatedFromFile = true;
                importer.Replace(book);
            }
        }
    }
}
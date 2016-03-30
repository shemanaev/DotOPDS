using DotOPDS.Models;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DotOPDS.Utils
{
    class FileUtils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Stream GetBookFile(Book book)
        {
            // TODO: support books without archives
            var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, book.Archive);
            if (!File.Exists(archive))
            {
                logger.Warn("Archive {0} not found.", archive);
                throw new KeyNotFoundException("File not found.");
            }
            var filename = string.Format("{0}.{1}", book.File, book.Ext);

            var zip = ZipFile.OpenRead(archive);
            {
                logger.Debug("Archive {0} opened", archive);

                var entry = zip.GetEntry(filename);
                if (entry == null)
                {
                    logger.Warn("File {0} in archive {1} not found.", filename, archive);
                    throw new KeyNotFoundException("File not found.");
                }

                return entry.Open();
            }
        }
    }
}

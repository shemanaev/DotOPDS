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
            var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, book.Archive);
            var filename = string.Format("{0}.{1}", book.File, book.Ext);
            if (File.Exists(archive))
            {
                return GetBookFileFromArchive(archive, filename);
            }
            
            var fullpath = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, filename);
            if (File.Exists(fullpath))
            {
                return File.OpenRead(fullpath);
            }

            throw new KeyNotFoundException("File not found.");
        }

        private static Stream GetBookFileFromArchive(string archive, string filename)
        {
            logger.Trace("Trying to open archive, {0}", archive);
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

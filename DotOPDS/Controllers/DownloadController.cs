using DotOPDS.Models;
using DotOPDS.Utils;
using Ionic.Zip;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DotOPDS.Controllers
{
    public class DownloadController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Route("get/{id:guid}/{ext}")]
        [HttpGet]
        public HttpResponseMessage GetFile(Guid id, string ext)
        {
            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Guid", id.ToString(), take: 1);
            if (total < 1 || total > 1)
            {
                logger.Debug("File {0} not found", id);
                throw new KeyNotFoundException("Key Not Found: " + id);
            }

            logger.Debug("File {0} found in {1}ms", id, searcher.Time);

            var book = books[0];
            var converter = Settings.Instance.Converters
                .Where(x => x.From.Equals(book.Ext, StringComparison.InvariantCultureIgnoreCase) &&
                            x.To.Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
            if (!ext.Equals(book.Ext, StringComparison.InvariantCultureIgnoreCase) && converter == null)
            {
                logger.Warn("No converter found for '{0}'->'{1}'", book.Ext, ext);
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var content = GetFileInFormat(book, converter);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var mimeType = MimeHelper.GetMimeType(ext);
            result.Content = new StreamContent(content);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = UrlNameEncoder.GetSafeName(book, ext);

            logger.Info("Book {0} served to {1}", id, Request.GetOwinContext().Request.RemoteIpAddress);
            return result;
        }

        private Stream GetFileInFormat(Book book, SettingsConverter converter)
        {
            // TODO: support books without archives
            var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, book.Archive);
            if (!File.Exists(archive))
            {
                logger.Warn("Archive {0} not found.", archive);
                throw new KeyNotFoundException("File not found.");
            }
            var filename = string.Format("{0}.{1}", book.File, book.Ext);

            var zip = ZipFile.Read(archive);
            Request.RegisterForDispose(zip);
            logger.Debug("Archive {0} opened", archive);
            if (!zip.ContainsEntry(filename))
            {
                logger.Warn("File {0} in archive {1} not found.", filename, archive);
                throw new KeyNotFoundException("File not found.");
            }

            zip.FlattenFoldersOnExtract = true;
            var entry = zip[filename];

            // TODO: zip fb2 book?
            if (converter == null)
            {
                logger.Debug("File {0} served directly from archive.", filename);
                return entry.OpenReader();
            }
            else
            {
                logger.Debug("Trying to convert file {0} from {1} to {2}", filename, book.Ext, converter.To);

                var tempDir = Path.Combine(Path.GetTempPath(), "DotOPDS_Temp");
                Directory.CreateDirectory(tempDir);
                var from = Path.Combine(tempDir, Path.GetRandomFileName() + filename);
                var to = Path.Combine(tempDir, Path.GetRandomFileName()) + "." + converter.To;
                entry.Extract(from, ExtractExistingFileAction.DoNotOverwrite);

                var cmd = string.Format(converter.Command, from, to);
                logger.Debug("Starting converter process: {0}", cmd);
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {

                        FileName = "cmd.exe",
                        Arguments = "/c " + cmd,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
                logger.Debug("Converter process exited with code {0}", process.ExitCode);

                return File.OpenRead(to);
            }
        }
    }
}

using DotOPDS.Models;
using DotOPDS.Utils;
using Ionic.Zip;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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
            var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, book.Archive);
            if (!File.Exists(archive))
            {
                logger.Warn("Archive {0} not found.", archive);
                throw new KeyNotFoundException("Key Not Found: " + id);
            }
            var filename = string.Format("{0}.{1}", book.File, book.Ext);

            var zip = ZipFile.Read(archive);
            Request.RegisterForDispose(zip);
            logger.Debug("Archive {0} opened", archive);
            if (!zip.ContainsEntry(filename))
            {
                logger.Warn("File {0} in archive {1} not found.", filename, archive);
                throw new KeyNotFoundException("Key Not Found: " + id);
            }

            var entry = zip[filename];

            // TODO: handle extensions for converting
            // TODO: zip fb2 book?
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var pushStreamContent = new PushStreamContent((stream, content, context) =>
            {
                try
                {
                    entry.Extract(stream);
                }
                finally
                {
                    logger.Debug("Cleaned up after {0} file download", id);
                    logger.Info("Book {0} served to {1}", id, Request.GetOwinContext().Request.RemoteIpAddress);
                    stream.Close();
                    zip.Dispose();
                }
            });
            result.Content = pushStreamContent;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(FeedLinkType.Fb2);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = UrlNameEncoder.GetSafeName(book, ext);

            logger.Debug("File {0} sent", id);
            return result;
        }
    }
}

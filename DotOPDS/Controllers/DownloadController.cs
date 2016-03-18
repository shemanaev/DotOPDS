using DotOPDS.Utils;
using Ionic.Zip;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace DotOPDS.Controllers
{
    public class DownloadController : WebApiController
    {
        [WebApiHandler(HttpVerbs.Get, "/download/*")]
        public bool GetFile(WebServer server, HttpListenerContext context)
        {
            try
            {
                var name = context.Request.Url.Segments.Last();
                var guid = context.Request.Url.Segments.Reverse().Skip(1).Take(1).SingleOrDefault().TrimEnd('/');
                if (guid.EndsWith("/"))
                    throw new KeyNotFoundException("Key Not Found: " + guid);


                var searcher = new LuceneSearcher();
                int total;
                var books = searcher.SearchExact(out total, "Guid", guid, take: 1);
                if (total < 1 || total > 1)
                {
                    throw new KeyNotFoundException("Key Not Found: " + guid);
                }
                Log.Debug("search done {0}", guid);
                var book = books[0];
                var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId].Path, book.Archive);
                if (!File.Exists(archive))
                {
                    Log.Warning("Archive {0} not found.", archive);
                    throw new KeyNotFoundException("Key Not Found: " + guid);
                }
                var filename = string.Format("{0}.{1}", book.File, book.Ext);
                Log.Debug("archive before opened {0}", archive);
                using (var zip = ZipFile.Read(archive))
                {
                    Log.Debug("archive opened {0}", archive);
                    if (!zip.ContainsEntry(filename))
                    {
                        Log.Warning("File {0} in archive {1} not found.", filename, archive);
                        throw new KeyNotFoundException("Key Not Found: " + guid);
                    }
                    var entry = zip[filename];
                    Log.Debug("entry opened {0}", entry);
                    // TODO: handle extensions for converting
                    //context.Response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=""{0}""", name));
                    entry.Extract(context.Response.OutputStream);
                    return true;
                }

                throw new KeyNotFoundException("Key Not Found: " + guid);
            }
            catch (Exception ex)
            {
                return HandleError(context, ex, (int)HttpStatusCode.InternalServerError);
            }
        }

        protected bool HandleError(HttpListenerContext context, Exception ex, int statusCode = 500)
        {
            var errorResponse = new
            {
                Title = "Unexpected Error",
                ErrorCode = ex.GetType().Name,
                Description = ex.ExceptionMessage(),
            };

            context.Response.StatusCode = statusCode;
            return context.JsonResponse(errorResponse);
        }

    }
}

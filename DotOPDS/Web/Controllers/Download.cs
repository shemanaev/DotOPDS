using Ionic.Zip;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Nancy;
using Nancy.Responses;
using Serilog;
using System.IO;

namespace DotOPDS.Web.Controllers
{
    public class Download : BaseRoute
    {
        public Download()
        {
            Get["/download/{id:guid}/{name}.{extension}"] = x =>
            {
                var query = new BooleanQuery();
                var phraseQuery = new PhraseQuery();
                phraseQuery.Add(new Term("Guid", x.id));
                query.Add(phraseQuery, Occur.MUST);

                int total;
                var books = Search(query, out total);
                if (total < 1 || total > 1)
                {
                    return Negotiate.WithStatusCode(404);
                }
                Log.Debug("search done {0}", x.id);
                var book = books[0];
                var archive = Path.Combine(Settings.Instance.Libraries[book.LibraryId], book.Book.Archive);
                if (!File.Exists(archive))
                {
                    Log.Warning("Archive {0} not found.", archive);
                    return Negotiate.WithStatusCode(404);
                }
                var filename = string.Format("{0}.{1}", book.Book.File, book.Book.Ext);
                Log.Debug("archive before opened {0}", archive);
                using (var zip = ZipFile.Read(archive))
                {
                    Log.Debug("archive opened {0}", archive);
                    if (!zip.ContainsEntry(filename))
                    {
                        Log.Warning("File {0} in archive {1} not found.", filename, archive);
                        return Negotiate.WithStatusCode(404);
                    }
                    var entry = zip[filename];
                    Log.Debug("entry opened {0}", entry);
                    using (var stream = new MemoryStream())
                    {
                        entry.Extract(stream);
                        return new StreamResponse(() => stream, "application/octet-stream");

                    }
                }
            };
        }
    }
}

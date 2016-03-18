using DotOPDS.Models;
using DotOPDS.Utils;
using System.Linq;
using System.Net;
using System.Web;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace DotOPDS.Controllers
{
    class SearchController : WebApiController
    {
        private const string RelativePath = "/opds";
        private static FeedLink SearchLink = new FeedLink { Rel = "search", Type = FeedLinkType.Atom, Href = RelativePath + "/search?q={searchTerms}&amp;page={startPage?}" };
        private static FeedLink StartLink = new FeedLink { Rel = "start", Type = FeedLinkType.AtomNavigation, Href = RelativePath };

        [WebApiHandler(HttpVerbs.Get, RelativePath)]
        public bool Index(WebServer server, HttpListenerContext context)
        {
             var feed = new Feed();
             feed.Id = "tag:root:root";
             feed.Title = "DotOPDS catalog";
             AddNavigation(context, feed);

             var entry = new FeedEntry();
             entry.Id = "tag:root:authors";
             entry.Title = "Книги по авторам";
             entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = RelativePath + "/authors" });
             entry.Content = new FeedEntryContent { Text = "Просмотр книг по авторам" };
             feed.Entries.Add(entry);

             entry = new FeedEntry();
             entry.Id = "tag:root:series";
             entry.Title = "Книги по сериям";
             entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = RelativePath + "/series" });
             entry.Content = new FeedEntryContent { Text = "Просмотр книг по сериям" };
             feed.Entries.Add(entry);

             entry = new FeedEntry();
             entry.Id = "tag:root:genres";
             entry.Title = "Книги по жанрам";
             entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = RelativePath + "/genres" });
             entry.Content = new FeedEntryContent { Text = "Просмотр книг по жанрам" };
             feed.Entries.Add(entry);

             return context.FeedResponse(feed);
        }

        [WebApiHandler(HttpVerbs.Get, RelativePath + "/search")]
        public bool Search(WebServer server, HttpListenerContext context)
        {
            var page = int.Parse(context.Query("page") ?? "1");
            var q = context.Query("q");

            var searcher = new LuceneSearcher();
            int total;
            var query = string.Format(@"Title:""{0}"" OR Author:""{0}"" OR Series:""{0}""", searcher.Escape(q));
            var books = searcher.Search(out total, query, "Title", page);

            var feed = new Feed();
            feed.Id = "tag:root:search:" + q;
            feed.Title = "Search results " + q;
            feed.Total = total;
            AddNavigation(context, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return context.FeedResponse(feed);
        }

        [WebApiHandler(HttpVerbs.Get, RelativePath + "/genre/*")]
        public bool SearchByGenre(WebServer server, HttpListenerContext context)
        {
            var page = int.Parse(context.Query("page") ?? "1");
            var genre = context.Request.Url.Segments.Last().TrimEnd('/');

            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Genre", genre, page);

            var feed = new Feed();
            feed.Id = "tag:root:genre:" + genre;
            feed.Title = "Book by genre " + genre;
            feed.Total = total;
            AddNavigation(context, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return context.FeedResponse(feed);
        }

        [WebApiHandler(HttpVerbs.Get, RelativePath + "/author/*")]
        public bool SearchByAuthor(WebServer server, HttpListenerContext context)
        {
            var page = int.Parse(context.Query("page") ?? "1");
            var author = context.Request.Url.Segments.Last().TrimEnd('/');

            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Author.Exact", author, page);

            var feed = new Feed();
            feed.Id = "tag:root:author:" + author;
            feed.Title = "Book by author " + author;
            feed.Total = total;
            AddNavigation(context, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return context.FeedResponse(feed);
        }

        [WebApiHandler(HttpVerbs.Get, RelativePath + "/series/*")]
        public bool SearchBySeries(WebServer server, HttpListenerContext context)
        {
            var page = int.Parse(context.Query("page") ?? "1");
            var series = context.Segments().Last().TrimEnd('/');

            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Series.Exact", series, page);

            var feed = new Feed();
            feed.Id = "tag:root:series:" + series;
            feed.Title = "Book by series " + series;
            feed.Total = total;

            AddNavigation(context, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return context.FeedResponse(feed);
        }

        private void AddNavigation(HttpListenerContext context, Feed feed, int page = 0, int total = 0, LuceneSearcher searcher = null)
        {
            feed.Links.Add(SearchLink);
            feed.Links.Add(StartLink);
            feed.Links.Add(new FeedLink { Rel = "self", Type = FeedLinkType.AtomNavigation, Href = context.Request.RawUrl });
            if (page > 1)
            {
                var url = context.Request.RawUrl;
                if (page - 1 != 1)
                {
                    var newPage = "page=" + (page - 1);

                    if (url.Contains("page=")) url = url.Replace("page=" + page, newPage);
                    else if (context.Request.Url.Query.Contains("?")) url += "&" + newPage;
                    else url += "?" + newPage;
                }
                else
                {
                    url = url.Replace("&page=" + page, "");
                }
                feed.Links.Add(new FeedLink
                {
                    Rel = "prev",
                    Type = FeedLinkType.AtomNavigation,
                    Href = url
                });
            }
            if (page * Settings.Instance.Pagination < total)
            {
                var newPage = "page=" + (page + 1);
                var url = context.Request.RawUrl;
                if (url.Contains("page=")) url = url.Replace("page=" + page, newPage);
                else if (context.Request.Url.Query.Contains("?")) url += "&" + newPage;
                else url += "?" + newPage;
                feed.Links.Add(new FeedLink
                {
                    Rel = "next",
                    Type = FeedLinkType.AtomNavigation,
                    Href = url
                });
            }
#if DEBUG
            if (searcher != null)
            {
                feed.Links.Add(new FeedLink { Rel = "debug", Type = "query", Title = searcher.Query });
                feed.Links.Add(new FeedLink { Rel = "debug", Type = "time", Title = searcher.Time + "ms" });
            }
#endif
        }

        private string GetAuthorName(Author author)
        {
            string format = "";
            if (!string.IsNullOrEmpty(author.FirstName)) format += "{0}";
            if (!string.IsNullOrEmpty(author.MiddleName)) format += " {1}";
            if (!string.IsNullOrEmpty(author.LastName)) format += " {2}";
            return string.Format(format, author.FirstName, author.MiddleName, author.LastName).Trim();
        }

        private FeedEntry GetBook(Book book)
        {
            var entry = new FeedEntry();
            var filename = book.Title;
            if (book.Authors.Length > 0)
            {
                filename = string.Format("{0}-{1}", GetAuthorName(book.Authors[0]), filename);
            }
            filename = UrlNameEncoder.Encode(filename);

            entry.Id = string.Format("tag:book:{0}", book.Id);
            entry.Title = book.Title;

            foreach (var author in book.Authors)
            {
                var name = GetAuthorName(author);
                entry.Authors.Add(new FeedAuthor
                {
                    Name = name,
                    Uri = string.Format(RelativePath + "/author/{0}", HttpUtility.UrlEncode(name))
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.Atom,
                    Href = string.Format(RelativePath + "/author/{0}", HttpUtility.UrlEncode(name)),
                    Title = string.Format("Все книги автора {0}", name)
                });
            }

            foreach (var genre in book.Genres)
            {
                entry.Categories.Add(new FeedCategory
                {
                    Label = genre,
                    Term = genre
                });
            }

            if (!string.IsNullOrWhiteSpace(book.Series))
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = "related",
                    Type = FeedLinkType.Atom,
                    Href = string.Format(RelativePath + "/series/{0}", HttpUtility.UrlEncode(book.Series)),
                    Title = string.Format(@"Все книги серии ""{0}""", book.Series)
                });
            }

            entry.Links.Add(new FeedLink
            {
                Rel = "http://opds-spec.org/acquisition/open-access",
                Type = FeedLinkType.Fb2,
                Href = string.Format("/download/{0}/{1}.fb2.zip", book.Id, filename)
            });

            foreach (var converter in Settings.Instance.Converters)
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = "http://opds-spec.org/acquisition/open-access",
                    Type = string.Format("application/{0}+zip", converter.Key),
                    Href = string.Format("/download/{0}/{1}.{2}", book.Id, filename, converter.Key)
                });
            }
            /*
<link href="/i/3/427403/_0.jpg" rel="http://opds-spec.org/image" type="image/jpeg" />
<link href="/i/3/427403/_0.jpg" rel="x-stanza-cover-image" type="image/jpeg" />
<link href="/i/3/427403/_0.jpg" rel="http://opds-spec.org/thumbnail" type="image/jpeg" />
<link href="/i/3/427403/_0.jpg" rel="x-stanza-cover-image-thumbnail" type="image/jpeg" />
<link href="/b/427403" rel="alternate" type="text/html" title="Книга на сайте" />
*/
            return entry;
        }
    }
}

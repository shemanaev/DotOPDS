using DotOPDS.Covers;
using DotOPDS.Models;
using DotOPDS.Utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace DotOPDS.Controllers
{
    public class SearchController : ApiController
    {
        private static FeedLink SearchLink = new FeedLink { Rel = FeedLinkRel.Search, Type = FeedLinkType.Atom, Href = "/search?q={searchTerms}&amp;page={startPage?}" };
        private static FeedLink StartLink = new FeedLink { Rel = FeedLinkRel.Start, Type = FeedLinkType.AtomNavigation, Href = "/" };

        [Route("")]
        [HttpGet]
        public Feed Index()
        {
            var feed = new Feed();
            feed.Id = "tag:root:root";
            feed.Title = Settings.Instance.Title;
            AddNavigation(Request.RequestUri, feed);

            /* TODO: add alphabet indexes by authors and series
            var entry = new FeedEntry();
            entry.Id = "tag:root:authors";
            entry.Title = T._("Books by authros"); // Книги по авторам
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = "/authors" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by authors") }; // Просмотр книг по авторам
            feed.Entries.Add(entry);

            entry = new FeedEntry();
            entry.Id = "tag:root:series";
            entry.Title = T.("Books by series"); // Книги по сериям
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = "/series" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by series") }; // Просмотр книг по сериям
            feed.Entries.Add(entry);
            */
            var entry = new FeedEntry();
            entry.Id = "tag:root:genres";
            entry.Title = T._("Books by genres"); // Книги по жанрам
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = "/genres" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by genres") }; // Просмотр книг по жанрам
            feed.Entries.Add(entry);

            return feed;
        }

        [Route("search")]
        [HttpGet]
        public Feed Search([FromUri] string q, [FromUri] int page = 1)
        {
            var searcher = new LuceneSearcher();
            int total;
            var query = string.Format(@"Title:""{0}"" OR Author:""{0}"" OR Series:""{0}""", searcher.Escape(q));
            var books = searcher.Search(out total, query, "Title", page);

            var feed = new Feed();
            feed.Id = "tag:root:search:" + q;
            feed.Title = T._("Search results: {0}", q);
            feed.Total = total;
            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("genre/{genre}")]
        [HttpGet]
        public Feed SearchByGenre(string genre, [FromUri] int page = 1)
        {
            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Genre", genre, page);

            var feed = new Feed();
            feed.Id = "tag:root:genre:" + genre;
            feed.Title = T._("Books in the genre of {0}", Genres.Instance.Localize(genre));
            feed.Total = total;
            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("search")]
        [HttpGet]
        public Feed SearchByAuthor([FromUri] string author, [FromUri] int page = 1)
        {
            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Author.Exact", author, page);

            var feed = new Feed();
            feed.Id = "tag:root:author:" + author;
            feed.Title = T._("Books by {0}", author);
            feed.Total = total;
            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("search")]
        [HttpGet]
        public Feed SearchBySeries([FromUri] string series, [FromUri] int page = 1)
        {
            var searcher = new LuceneSearcher();
            int total;
            var books = searcher.SearchExact(out total, "Series.Exact", series, page);

            var feed = new Feed();
            feed.Id = "tag:root:series:" + series;
            feed.Title = T._("Books in the series {0}", series);
            feed.Total = total;

            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        private string ChangePage(string url, int page = 0)
        {
            string result = url;
            if (url.Contains("page="))
            {
                var r = @"([\?&]page=)(\d+)";
                var value = page <= 1 ? "" : "${1}" + page;
                result = Regex.Replace(url, r, value);
            }
            else
            {
                if (url.Contains("?")) result += "&";
                else result += "?";
                result += "page=" + page;
            }
            return result;
        }

        private void AddNavigation(Uri uri, Feed feed, int page = 0, int total = 0, LuceneSearcher searcher = null)
        {
            var favicon = Path.Combine(Util.Normalize(Settings.Instance.Web), "favicon.ico");
            if (File.Exists(favicon))
            {
                feed.Icon = "/static/favicon.ico";
            }

            feed.Links.Add(SearchLink);
            feed.Links.Add(StartLink);
            feed.Links.Add(new FeedLink { Rel = FeedLinkRel.Self, Type = FeedLinkType.AtomNavigation, Href = uri.PathAndQuery });
            if (page > 1)
            {
                feed.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Prev,
                    Type = FeedLinkType.AtomNavigation,
                    Href = ChangePage(uri.PathAndQuery, page - 1)
                });
            }
            if (page * Settings.Instance.Pagination < total)
            {
                feed.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Next,
                    Type = FeedLinkType.AtomNavigation,
                    Href = ChangePage(uri.PathAndQuery, page + 1)
                });
            }
#if DEBUG
            if (searcher != null)
            {
                feed.Links.Add(new FeedLink { Rel = FeedLinkRel.Debug, Type = "query", Title = searcher.Query });
                feed.Links.Add(new FeedLink { Rel = FeedLinkRel.Debug, Type = "time", Title = searcher.Time + "ms" });
            }
#endif
        }

        private FeedEntry GetBook(Book book)
        {
            var entry = new FeedEntry();
            var filename = book.Title;
            if (book.Authors.Length > 0)
            {
                filename = string.Format("{0} - {1}", book.Authors[0].GetScreenName(), filename);
            }
            filename = UrlNameEncoder.Encode(filename);

            entry.Id = string.Format("tag:book:{0}", book.Id);
            entry.Title = book.Title;

            foreach (var author in book.Authors)
            {
                var name = author.GetScreenName();
                entry.Authors.Add(new FeedAuthor
                {
                    Name = name,
                    Uri = string.Format("/search?author={0}", HttpUtility.UrlEncode(name))
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.AtomNavigation,
                    Href = string.Format("/search?author={0}", HttpUtility.UrlEncode(name)),
                    Title = T._("All books by {0}", name) // Все книги автора {0}
                });
            }

            foreach (var genre in book.Genres)
            {
                var local = Genres.Instance.Localize(genre);
                entry.Categories.Add(new FeedCategory
                {
                    Label = local,
                    Term = local
                });
            }

            if (!string.IsNullOrWhiteSpace(book.Series))
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.AtomNavigation,
                    Href = string.Format("/search?series={0}", HttpUtility.UrlEncode(book.Series)),
                    Title = T._("All books in the series") // Все книги из серии
                });
            }

            entry.Links.Add(new FeedLink
            {
                Rel = FeedLinkRel.Acquisition,
                Type = FeedLinkType.Fb2,
                Href = string.Format("/get/{0}/{1}.fb2", book.Id, filename)
            });

            foreach (var converter in Settings.Instance.Converters)
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Acquisition,
                    Type = string.Format("application/{0}+zip", converter.Key),
                    Href = string.Format("/get/{0}/{1}.{2}", book.Id, filename, converter.Key)
                });
            }

            var cover = CoverResolver.Instance.Get(book);
            if (!string.IsNullOrEmpty(cover))
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Image,
                    Type = FeedLinkType.Jpeg,
                    Href = cover
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Thumbnail,
                    Type = FeedLinkType.Jpeg,
                    Href = cover
                });
            }

            return entry;
        }
    }
}

using DotOPDS.Models;
using DotOPDS.Parsers;
using DotOPDS.Plugins;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace DotOPDS.Controllers
{
    [RoutePrefix("opds")]
    public class SearchController : ApiController
    {
        private const string Prefix = "/opds";
        private static FeedLink SearchLink = new FeedLink { Rel = FeedLinkRel.Search, Type = FeedLinkType.Atom, Href = Prefix + "/search?q={searchTerms}" };
        private static FeedLink StartLink = new FeedLink { Rel = FeedLinkRel.Start, Type = FeedLinkType.AtomNavigation, Href = Prefix.StartsWith("/") ? Prefix : "/" };
        private static readonly IndexField[] predefinedSearchFields =
        {
            new IndexField {Field = "title", DisplayName = "title"},
            new IndexField {Field = "author", DisplayName = "author"},
            new IndexField {Field = "series", DisplayName = "series"},
        };

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
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/authors" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by authors") }; // Просмотр книг по авторам
            feed.Entries.Add(entry);

            entry = new FeedEntry();
            entry.Id = "tag:root:series";
            entry.Title = T.("Books by series"); // Книги по сериям
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/series" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by series") }; // Просмотр книг по сериям
            feed.Entries.Add(entry);
            */
            var entry = new FeedEntry();
            entry.Id = "tag:root:genres";
            entry.Title = T._("Books by genres"); // Книги по жанрам
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/genres" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by genres") }; // Просмотр книг по жанрам
        [Route("search")]
        [HttpGet]
        [RequiredParameters]
        public Feed SearchIndex([FromUri] string q)
        {
            var feed = new Feed();
            feed.Id = $"tag:root:search:{q}";
            feed.Title = Settings.Instance.Title;
            AddNavigation(Request.RequestUri, feed);

            var entry = new FeedEntry();
            entry.Id = $"tag:root:search:everywhere:{q}";
            entry.Title = T._("Search everywhere");
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?everywhere={HttpUtility.UrlEncode(q)}" });
            entry.Content = new FeedEntryContent { Text = T._("Search in titles, authors and series") };
            feed.Entries.Add(entry);

            foreach (var f in predefinedSearchFields)
            {
                entry = new FeedEntry();
                entry.Id = $"tag:root:search:{f.Field}:{q}";
                entry.Title = T._("Search in {0}", f.DisplayName);
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?field={f.Field}&q={HttpUtility.UrlEncode(q)}" });
                entry.Content = new FeedEntryContent { Text = T._("Search in {0}", f.DisplayName) };
                feed.Entries.Add(entry);
            }

            foreach (var f in PluginProvider.Instance.IndexFields)
            {
                entry = new FeedEntry();
                entry.Id = $"tag:root:search:{f.Field}:{q}";
                entry.Title = T._("Search in {0}", f.DisplayName);
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?field={f.Field}&q={HttpUtility.UrlEncode(q)}" });
                entry.Content = new FeedEntryContent { Text = T._("Search in {0}", f.DisplayName) };
                feed.Entries.Add(entry);
            }

            entry = new FeedEntry();
            entry.Id = $"tag:root:search:advanced:{q}";
            entry.Title = T._("Advanced search");
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?advanced={HttpUtility.UrlEncode(q)}" });
            entry.Content = new FeedEntryContent { Text = T._("Advanced search") };
            feed.Entries.Add(entry);

            return feed;
        }

        [Route("search")]
        [HttpGet]
        [RequiredParameters]
        public Feed Search([FromUri] string everywhere, [FromUri] int page = 1)
        {
            if (page < 1) page = 1;
            var searcher = new LuceneIndexStorage();
            int total;
            var query = string.Format(@"title:""{0}"" OR author:""{0}"" OR series:""{0}""", searcher.Escape(everywhere));
            var books = searcher.Search(out total, query, "title", page);

            var feed = new Feed();
            feed.Id = $"tag:root:search:everywhere:{everywhere}";
            feed.Title = T._("Search results: {0}", everywhere);
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
        [RequiredParameters]
        public Feed SearchInField([FromUri] string q, [FromUri] string field, [FromUri] int page = 1)
        {
            if (page < 1) page = 1;
            var searcher = new LuceneIndexStorage();
            int total = 0;
            List<Book> books = null;

            if (predefinedSearchFields.Any(f => f.Field == field) || PluginProvider.Instance.IndexFields.Any(f => f.Field == field))
            {
                books = searcher.Search(out total, q, field, page);
            }

            var feed = new Feed();
            feed.Id = $"tag:root:search:{field}:{q}";
            feed.Title = T._("Search results: {0}", q);
            feed.Total = total;
            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            if (books != null)
                foreach (var book in books)
                {
                    feed.Entries.Add(GetBook(book));
                }

            return feed;
        }

        [Route("search")]
        [HttpGet]
        [RequiredParameters]
        public Feed SearchAdvanced([FromUri] string advanced, [FromUri] int page = 1)
        {
            if (page < 1) page = 1;
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.Search(out total, advanced, "title", page);

            var feed = new Feed();
            feed.Id = $"tag:root:search:advanced:{advanced}";
            feed.Title = T._("Search results: {0}", advanced);
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
            if (page < 1) page = 1;
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "genre", genre, page);

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

        [Route("author/{author}")]
        [HttpGet]
        [RequiredParameters]
        public Feed SearchByAuthor(string author, [FromUri] int page = 1)
        {
            if (page < 1) page = 1;
            author = HttpUtility.UrlDecode(author);
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "author_exact", author, page);

            var feed = new Feed();
            feed.Id = $"tag:root:author:{author}";
            feed.Title = T._("Books by {0}", author);
            feed.Total = total;
            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("series/{series}")]
        [HttpGet]
        [RequiredParameters]
        public Feed SearchBySeries(string series, [FromUri] int page = 1)
        {
            if (page < 1) page = 1;
            series = HttpUtility.UrlDecode(series);
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "series_exact", series, page);

            var feed = new Feed();
            feed.Id = $"tag:root:series:{series}";
            feed.Title = T._("Books in the series {0}", series);
            feed.Total = total;

            AddNavigation(Request.RequestUri, feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("genres")]
        [HttpGet]
        public Feed GetByGenres()
        {
            var feed = new Feed();
            feed.Id = "tag:root:genres";
            feed.Title = T._("Books by genres");
            AddNavigation(Request.RequestUri, feed);

            foreach (var genre in Genres.Instance.Tree)
            {
                var name = genre.Key.Replace("category_", "");
                var entry = new FeedEntry();
                entry.Id = "tag:root:genre:" + name;
                entry.Title = Genres.Instance.Localize(genre.Key);
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/genres/" + name });
                entry.Content = new FeedEntryContent { Text = T._("Books in the genre of {0}", Genres.Instance.Localize(genre.Key)) };
                feed.Entries.Add(entry);
            }

            return feed;
        }

        [Route("genres/{category}")]
        [HttpGet]
        public Feed GetByGenres(string category)
        {
            var fullname = "category_" + category;
            var feed = new Feed();
            feed.Id = "tag:root:genres:" + category;
            feed.Title = T._("Books in the genre of {0}", Genres.Instance.Localize(fullname));
            AddNavigation(Request.RequestUri, feed);

            foreach (var genre in Genres.Instance.Tree[fullname])
            {
                var entry = new FeedEntry();
                entry.Id = "tag:root:genre:" + genre;
                entry.Title = Genres.Instance.Localize(genre);
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/genre/" + genre });
                entry.Content = new FeedEntryContent { Text = T._("Books in the genre of {0}", Genres.Instance.Localize(genre)) };
                feed.Entries.Add(entry);
            }

            return feed;
        }

        private string ChangePage(string url, int page = 0)
        {
            string result = url;
            if (url.Contains("page="))
            {
                const string r = @"([\?&]page=)(\d*)";
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

        private void AddNavigation(Uri uri, Feed feed, int page = 0, int total = 0, LuceneIndexStorage searcher = null)
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

            entry.Id = string.Format("tag:book:{0}", book.Id);
            entry.Title = book.Title;
            entry.Issued = book.Date.Year;
            entry.Language = book.Language;
            entry.Updated = book.UpdatedAt.ToString("s");

            foreach (var author in book.Authors)
            {
                var name = author.GetScreenName();
                entry.Authors.Add(new FeedAuthor
                {
                    Name = name,
                    Uri = Prefix + string.Format("/author/{0}", HttpUtility.UrlEncode(name))
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.AtomNavigation,
                    Href = Prefix + string.Format("/author/{0}", HttpUtility.UrlEncode(name)),
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
                    Href = Prefix + string.Format("/series/{0}", HttpUtility.UrlEncode(book.Series)),
                    Title = T._("All books in the series") // Все книги из серии
                });

                entry.Series = new FeedEntrySeries
                {
                    Name = book.Series,
                    Number = book.SeriesNo
                };
            }

            entry.Links.Add(new FeedLink
            {
                Rel = FeedLinkRel.Acquisition,
                Type = MimeHelper.GetMimeType(book.Ext),
                Href = string.Format("/get/{0}/{1}", book.Id, book.Ext)
            });

            var converters = Settings.Instance.Converters
                .Where(x => x.From.Equals(book.Ext, StringComparison.InvariantCultureIgnoreCase));
            foreach (var converter in converters)
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Acquisition,
                    Type = MimeHelper.GetMimeType(converter.To),
                    Href = string.Format("/get/{0}/{1}", book.Id, converter.To)
                });
            }

            if (Settings.Instance.LazyInfoExtract)
                BookParsersPool.Instance.Update(book);

            if (book.Annotation != null)
            {
                entry.Content = new FeedEntryContent
                {
                    Type = "text/html",
                    Text = book.Annotation
                };
            }

            if (book.Cover != null)
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Image,
                    Type = book.Cover.ContentType,
                    Href = string.Format("/cover/{0}", book.Id)
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Thumbnail,
                    Type = book.Cover.ContentType,
                    Href = string.Format("/cover/{0}", book.Id)
                });
            }

            return entry;
        }
    }
}

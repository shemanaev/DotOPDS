using DotOPDS.Models;
using DotOPDS.Parsers;
using DotOPDS.Plugins;
using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            new IndexField {Field = "title", DisplayName = T._("title")},
            new IndexField {Field = "author", DisplayName = T._("author")},
            new IndexField {Field = "series", DisplayName = T._("series")},
        };

        [Route("")]
        [HttpGet]
        public Feed Index()
        {
            var feed = new Feed();
            feed.Id = "tag:root:root";
            feed.Title = Settings.Instance.Title;
            AddNavigation(Prefix, feed);

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
            entry.Title = T._("Books by genres");
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = Prefix + "/genres" });
            entry.Content = new FeedEntryContent { Text = T._("Browse books by genres") };
            feed.Entries.Add(entry);

            return feed;
        }

        [Route("search")]
        [HttpGet]
        [RequiredParameters]
        public Feed SearchIndex([FromUri] string q)
        {
            var feed = new Feed();
            feed.Id = $"tag:root:search:{q}";
            feed.Title = Settings.Instance.Title;
            AddNavigation($"{Prefix}/search?q={q}", feed);

            var entry = new FeedEntry();
            entry.Id = $"tag:root:search:everywhere:{q}";
            entry.Title = T._("Search everywhere");
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?everywhere={Uri.EscapeDataString(q)}" });
            entry.Content = new FeedEntryContent { Text = T._("Search in titles, authors and series") };
            feed.Entries.Add(entry);

            foreach (var f in predefinedSearchFields.Union(PluginProvider.Instance.IndexFields))
            {
                entry = new FeedEntry();
                entry.Id = $"tag:root:search:{f.Field}:{q}";
                entry.Title = T._("Search in {0}", f.DisplayName);
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?field={f.Field}&q={Uri.EscapeDataString(q)}" });
                entry.Content = new FeedEntryContent { Text = T._("Search in {0}", f.DisplayName) };
                feed.Entries.Add(entry);
            }

            entry = new FeedEntry();
            entry.Id = $"tag:root:search:advanced:{q}";
            entry.Title = T._("Advanced search");
            entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/search?advanced={Uri.EscapeDataString(q)}" });
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
            AddNavigation($"{Prefix}/search?everywhere={everywhere}", feed, page, total, searcher);

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
            AddNavigation($"{Prefix}/search?field={field}&q={q}", feed, page, total, searcher);

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
            AddNavigation($"{Prefix}/search?advanced={advanced}", feed, page, total, searcher);

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
            genre = Uri.UnescapeDataString(genre);
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "genre", genre, page);

            var feed = new Feed();
            feed.Id = "tag:root:genre:" + genre;
            feed.Title = T._("Books in the genre of {0}", GenreExtensions.GetDisplayName(genre));
            feed.Total = total;
            AddNavigation($"{Prefix}/genre/{Uri.EscapeDataString(genre)}", feed, page, total, searcher);

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
            author = Uri.UnescapeDataString(author);
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "author_exact", author, page);

            var feed = new Feed();
            feed.Id = $"tag:root:author:{author}";
            feed.Title = T._("Books by {0}", author);
            feed.Total = total;
            AddNavigation($"{Prefix}/author/{Uri.EscapeDataString(author)}", feed, page, total, searcher);

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
            series = Uri.UnescapeDataString(series);
            var searcher = new LuceneIndexStorage();
            int total;
            var books = searcher.SearchExact(out total, "series_exact", series, page);

            var feed = new Feed();
            feed.Id = $"tag:root:series:{series}";
            feed.Title = T._("Books in the series {0}", series);
            feed.Total = total;

            AddNavigation($"{Prefix}/series/{Uri.EscapeDataString(series)}", feed, page, total, searcher);

            foreach (var book in books)
            {
                feed.Entries.Add(GetBook(book));
            }

            return feed;
        }

        [Route("genres/{*genre}")]
        [HttpGet]
        public Feed GetByGenres(string genre)
        {
            genre = Uri.UnescapeDataString(genre ?? "");

            var feed = new Feed();
            feed.Id = "tag:root:genres" + (genre == "" ? "" : $":{genre}");
            feed.Title = genre == "" ? T._("Books by genres") : T._("Books in the genre of {0}", GenreExtensions.GetDisplayName(genre));
            AddNavigation($"{Prefix}/genres/{Uri.EscapeDataString(genre)}", feed);

            var searcher = new LuceneIndexStorage();
            var genres = searcher.GetAllGenres(genre);

            foreach (var k in genres)
            {
                var tag = k.Value.Item2 ? "genre" : "genres";
                var url = k.Value.Item1;
                var entry = new FeedEntry();
                entry.Id = $"tag:root:{tag}:{url}";
                entry.Title = k.Key;
                entry.Links.Add(new FeedLink { Type = FeedLinkType.AtomAcquisition, Href = $"{Prefix}/{tag}/{Uri.EscapeDataString(url)}" });
                entry.Content = new FeedEntryContent { Text = T._("Books in the genre of {0}", k.Key) };
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

        private void AddNavigation(string uri, Feed feed, int page = 0, int total = 0, LuceneIndexStorage searcher = null)
        {
            var favicon = Path.Combine(Util.Normalize(Settings.Instance.Web), "favicon.ico");
            if (File.Exists(favicon))
            {
                feed.Icon = "/favicon.ico";
            }

            feed.Links.Add(SearchLink);
            feed.Links.Add(StartLink);
            feed.Links.Add(new FeedLink { Rel = FeedLinkRel.Self, Type = FeedLinkType.AtomNavigation, Href = ChangePage(uri, page) });
            if (page > 1)
            {
                feed.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Prev,
                    Type = FeedLinkType.AtomNavigation,
                    Href = ChangePage(uri, page - 1)
                });
            }
            if (page * Settings.Instance.Pagination < total)
            {
                feed.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Next,
                    Type = FeedLinkType.AtomNavigation,
                    Href = ChangePage(uri, page + 1)
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
                    Uri = Prefix + string.Format("/author/{0}", Uri.EscapeDataString(name))
                });
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.AtomNavigation,
                    Href = Prefix + string.Format("/author/{0}", Uri.EscapeDataString(name)),
                    Title = T._("All books by {0}", name) // Все книги автора {0}
                });
            }

            foreach (var genre in book.Genres)
            {
                entry.Categories.Add(new FeedCategory
                {
                    Label = genre.GetDisplayName(),
                    Term = genre.GetFullName()
                });
            }

            if (!string.IsNullOrWhiteSpace(book.Series))
            {
                entry.Links.Add(new FeedLink
                {
                    Rel = FeedLinkRel.Related,
                    Type = FeedLinkType.AtomNavigation,
                    Href = Prefix + string.Format("/series/{0}", Uri.EscapeDataString(book.Series)),
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

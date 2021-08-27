using DotOPDS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using DotOPDS.Shared.Options;
using DotOPDS.Shared;
using DotOPDS.Shared.Plugins;
using DotOPDS.Contract.Plugins;
using DotOPDS.Contract.Models;
using DotOPDS.Extensions;
using DotOPDS.Dto;
using DotOPDS.Contract;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace DotOPDS.Controllers;

[Route("opds")]
public class OpdsController : ControllerBase
{
    private readonly PresentationOptions _options;
    private readonly LuceneIndexStorage _textSearch;
    private readonly BookParsersPool _bookParsers;
    private readonly PluginProvider _pluginProvider;
    private readonly MimeHelper _mimeHelper;
    private readonly ConverterService _converter;
    private readonly ITranslator T;
    private readonly IndexField[] _predefinedSearchFields;

    public OpdsController(
        IOptions<PresentationOptions> options,
        LuceneIndexStorage textSearch,
        BookParsersPool bookParsers,
        PluginProvider pluginProvider,
        MimeHelper mimeHelper,
        ConverterService converter,
        IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _textSearch = textSearch;
        _bookParsers = bookParsers;
        _pluginProvider = pluginProvider;
        _mimeHelper = mimeHelper;
        _converter = converter;

        var requestCulture = httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
        var language = requestCulture?.RequestCulture.Culture ?? new CultureInfo("en-US");
        T = Translator.GetForRequest(language);
        _predefinedSearchFields = new[]
        {
            new IndexField { Field = "title", DisplayName = T._("title") },
            new IndexField { Field = "author", DisplayName = T._("author") },
            new IndexField { Field = "series", DisplayName = T._("series") },
        };
    }

    [HttpGet(Name = nameof(Index))]
    public Feed Index()
    {
        var entries = new List<FeedEntry>();

        var entry = new FeedEntry("tag:root:genres", T._("Books by genres"), T._("Browse books by genres"));
        entry.Links.Add(new FeedLink(Url.RouteUrl(nameof(GetByGenres), null), FeedLinkType.AtomAcquisition));

        entries.Add(entry);

        return CreateFeed("root", _options.Title, Url.RouteUrl(nameof(Index), null), entries);
    }

    [Route("search", Name = nameof(SearchIndex))]
    public Feed SearchIndex([FromQuery] string q)
    {
        var entries = new List<FeedEntry>();

        var entry = new FeedEntry($"tag:root:search:everywhere:{q}", T._("Search everywhere"), T._("Search in titles, authors and series"));
        entry.Links.Add(new FeedLink(Url.RouteUrl(nameof(Search), new { q }), FeedLinkType.AtomAcquisition));
        entries.Add(entry);

        foreach (var f in _predefinedSearchFields.Union(_pluginProvider.IndexFields))
        {
            entry = new FeedEntry($"tag:root:search:{f.Field}:{q}", T._("Search in {0}", f.DisplayName), T._("Search in {0}", f.DisplayName));
            entry.Links.Add(new FeedLink(Url.RouteUrl(nameof(SearchInField), new { q, field = f.Field }), FeedLinkType.AtomAcquisition));
            entries.Add(entry);
        }

        entry = new FeedEntry($"tag:root:search:advanced:{q}", T._("Advanced search"), T._("Advanced search"));
        entry.Links.Add(new FeedLink(Url.RouteUrl(nameof(SearchAdvanced), new { q }), FeedLinkType.AtomAcquisition));
        entries.Add(entry);

        return CreateFeed($"search:{q}", _options.Title, Url.RouteUrl(nameof(SearchIndex), new { q }), entries);
    }

    [Route("search/everywhere", Name = nameof(Search))]
    public async Task<Feed> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var query = string.Format(@"title:""{0}"" OR author:""{0}"" OR series:""{0}""", LuceneIndexStorage.Escape(q));
        var books = _textSearch.Search(out int total, query, "title", page);

        var entries = new List<FeedEntry>();

        foreach (var book in books)
        {
            entries.Add(await GetBook(book, cancellationToken));
        }

        return CreateFeed($"search:everywhere:{q}",
            T._("Search results: {0}", q),
            Url.RouteUrl(nameof(Search), new { q }),
            entries, page, total);
    }

    [Route("search/byField", Name = nameof(SearchInField))]
    public async Task<Feed> SearchInField(
        [FromQuery] string q,
        [FromQuery] string field,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        int total = 0;
        List<Book>? books = null;

        if (_predefinedSearchFields.Any(f => f.Field == field) || _pluginProvider.IndexFields.Any(f => f.Field == field))
        {
            books = _textSearch.Search(out total, q, field, page);
        }

        var entries = new List<FeedEntry>();

        if (books != null)
        {
            foreach (var book in books)
            {
                entries.Add(await GetBook(book, cancellationToken));
            }
        }

        return CreateFeed($"search:{field}:{q}",
            T._("Search results: {0}", q),
            Url.RouteUrl(nameof(SearchInField), new { q, field }),
            entries, page, total);
    }

    [Route("search/advanced", Name = nameof(SearchAdvanced))]
    public async Task<Feed> SearchAdvanced(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var books = _textSearch.Search(out int total, q, "title", page);

        var entries = new List<FeedEntry>();

        foreach (var book in books)
        {
            entries.Add(await GetBook(book, cancellationToken));
        }

        return CreateFeed($"search:advanced:{q}",
            T._("Search results: {0}", q),
            Url.RouteUrl(nameof(SearchAdvanced), new { q }),
            entries, page, total);
    }

    [Route("genre/{genre}", Name = nameof(SearchByGenre))]
    public async Task<Feed> SearchByGenre(
        string genre,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var books = _textSearch.SearchExact(out int total, "genre", genre, page);

        var entries = new List<FeedEntry>();

        foreach (var book in books)
        {
            entries.Add(await GetBook(book, cancellationToken));
        }

        return CreateFeed($"genre:{genre}",
            T._("Books in the genre of {0}", GenreExtensions.GetDisplayName(genre)),
            Url.RouteUrl(nameof(SearchByGenre), new { genre }),
            entries, page, total);
    }

    [Route("author/{author}", Name = nameof(SearchByAuthor))]
    public async Task<Feed> SearchByAuthor(
        string author, 
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var books = _textSearch.SearchExact(out int total, "author_exact", author, page);

        var entries = new List<FeedEntry>();

        foreach (var book in books)
        {
            entries.Add(await GetBook(book, cancellationToken));
        }

        return CreateFeed($"author:{author}",
            T._("Books by {0}", author),
            Url.RouteUrl(nameof(SearchByAuthor), new { author }),
            entries, page, total);
    }

    [Route("series/{series}", Name = nameof(SearchBySeries))]
    public async Task<Feed> SearchBySeries(
        string series, 
        [FromQuery] int page = 1, 
        CancellationToken cancellationToken = default)
    {
        var books = _textSearch.SearchExact(out int total, "series_exact", series, page);

        var entries = new List<FeedEntry>();

        foreach (var book in books)
        {
            entries.Add(await GetBook(book, cancellationToken));
        }

        return CreateFeed($"series:{series}",
            T._("Books in the series {0}", series),
            Url.RouteUrl(nameof(SearchBySeries), new { series }),
            entries, page, total);
    }

    [Route("genres/{*genre}", Name = nameof(GetByGenres))]
    public Feed GetByGenres(string genre = "")
    {
        var entries = new List<FeedEntry>();

        genre ??= "";
        var genres = _textSearch.GetAllGenres(genre);

        foreach (var k in genres)
        {
            var tag = k.Value.Item2 ? "genre" : "genres";
            var link = Url.RouteUrl(k.Value.Item2 ? nameof(SearchByGenre) : nameof(GetByGenres), new { genre = k.Value.Item1 });
            var url = k.Value.Item1;
            var entry = new FeedEntry($"tag:root:{tag}:{url}", k.Key, T._("Books in the genre of {0}", k.Key));
            entry.Links.Add(new FeedLink(link, FeedLinkType.AtomAcquisition));
            entries.Add(entry);
        }

        return CreateFeed("genres" + (genre == "" ? "" : $":{genre}"),
            genre == "" ? T._("Books by genres") : T._("Books in the genre of {0}", GenreExtensions.GetDisplayName(genre)),
            Url.RouteUrl(nameof(GetByGenres), new { genre }),
            entries, 0, genres.Count);
    }

    private static string ChangePage(string url, int page)
    {
        var result = new StringBuilder(url);
        if (page > 1)
        {
            if (url.Contains('?'))
                result.Append($"&page={page}");
            else
                result.Append($"?page={page}");
        }
        return result.ToString();
    }

    private Feed CreateFeed(string id, string title, string uri, List<FeedEntry> entries, int page = 0, int total = -1)
    {
        var feed = new Feed
        {
            Id = $"tag:root:{id}",
            Title = title,
            ItemsPerPage = Math.Max(_options.PageSize, entries.Count),
            Icon = "/favicon.ico",
            Entries = entries,
            Total = total == -1 ? entries.Count : total,
        };

        // var favicon = Path.Combine(Util.Normalize(Settings.Instance.Web), "favicon.ico");
        // if (System.IO.File.Exists(favicon))
        // {
        //     feed.Icon = "/favicon.ico";
        // }

        feed.Links.Add(new FeedLink(Url.RouteUrlRaw(nameof(Search), new { q = "{searchTerms}" }), FeedLinkRel.Search, FeedLinkType.Atom));
        feed.Links.Add(new FeedLink(Url.RouteUrl(nameof(Index), null), FeedLinkRel.Start, FeedLinkType.AtomNavigation));
        feed.Links.Add(new FeedLink(ChangePage(uri, page), FeedLinkRel.Self, FeedLinkType.AtomNavigation));
        if (page > 1)
        {
            feed.Links.Add(new FeedLink(ChangePage(uri, page - 1), FeedLinkRel.Prev, FeedLinkType.AtomNavigation));
        }
        if (page * _options.PageSize < total)
        {
            feed.Links.Add(new FeedLink(ChangePage(uri, page + 1), FeedLinkRel.Next, FeedLinkType.AtomNavigation));
        }
#if DEBUG
        feed.Links.Add(new FeedLink("", FeedLinkRel.Debug, FeedLinkType.DebugQuery, _textSearch.Query));
        feed.Links.Add(new FeedLink("", FeedLinkRel.Debug, FeedLinkType.DebugTime, _textSearch.Time + "ms"));
#endif
        return feed;
    }

    private async Task<FeedEntry> GetBook(Book book, CancellationToken cancellationToken)
    {
        var entry = new FeedEntry
        {
            Id = string.Format("tag:book:{0}", book.Id),
            Title = book.Title,
            Issued = book.Date.Year,
            Language = book.Language,
            Updated = book.UpdatedAt.ToString("s")
        };

        foreach (var author in book.Authors)
        {
            var name = author.GetScreenName();
            var link = Url.RouteUrl(nameof(SearchByAuthor), new { author = name });
            entry.Authors.Add(new FeedAuthor(name, link));
            entry.Links.Add(new FeedLink(link, FeedLinkRel.Related, FeedLinkType.AtomNavigation, T._("All books by {0}", name))); // Все книги автора {0}
        }

        foreach (var genre in book.Genres)
        {
            entry.Categories.Add(new FeedCategory(genre.GetFullName(), genre.GetDisplayName()));
        }

        if (!string.IsNullOrWhiteSpace(book.Series))
        {
            var link = Url.RouteUrl(nameof(SearchBySeries), new { series = book.Series });
            entry.Links.Add(new FeedLink(link, FeedLinkRel.Related, FeedLinkType.AtomNavigation, T._("All books in the series"))); // Все книги из серии

            entry.Series = new FeedEntrySeries(book.Series, book.SeriesNo);
        }

        entry.Links.Add(new FeedLink(_mimeHelper.GetContentType(book.Ext),
            Url.RouteUrl(nameof(DownloadController.GetFile), new { id = book.Id, ext = book.Ext }),
            FeedLinkRel.Acquisition,
            FeedLinkType.Mime));

        foreach (var ext in _converter.GetAvailableConvertersForExt(book.Ext))
        {
            entry.Links.Add(new FeedLink(_mimeHelper.GetContentType(ext),
                Url.RouteUrl(nameof(DownloadController.GetFile), new { id = book.Id, ext = ext }),
                FeedLinkRel.Acquisition,
                FeedLinkType.Mime));
        }

        if (_options.LazyInfoExtract)
            await _bookParsers.Update(book, cancellationToken);

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
            entry.Links.Add(new(book.Cover.ContentType!,
                Url.RouteUrl(nameof(DownloadController.GetCover), new { id = book.Id }),
                FeedLinkRel.Image,
                FeedLinkType.Mime));
            entry.Links.Add(new(book.Cover.ContentType!,
                Url.RouteUrl(nameof(DownloadController.GetCover), new { id = book.Id }),
                FeedLinkRel.Thumbnail,
                FeedLinkType.Mime));
        }

        return entry;
    }
}

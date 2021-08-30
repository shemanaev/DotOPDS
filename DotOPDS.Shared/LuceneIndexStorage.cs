using DotOPDS.Contract.Models;
using DotOPDS.Extensions;
using DotOPDS.Shared.Options;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DotOPDS.Shared;

public class LuceneIndexStorage : IDisposable
{
    const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
    public const int VERSION = 1;

    private static readonly string[] KNOWN_FIELDS = 
    {
        "_updated_at",
        "_updated_from_file",
        "_unique_id",
        "guid",
        "_library_id",
        "title",
        "title_sort",
        "series",
        "series_exact",
        "seriesno",
        "file",
        "ext",
        "pubdate",
        "archive",
        "annotation",
        "_cover_type",
        "_cover_data",
        "author",
        "author_exact",
        "author_fullname",
        "genre",
        "language",
    };

    private readonly PresentationOptions _presentationOptions;
    private readonly IndexStorageOptions _storageOptions;
    private readonly string _indexPath;

    private IndexWriter writer;
    private Analyzer analyzer;
    private FSDirectory directory;
    private Stopwatch watch = Stopwatch.StartNew();

    public long Time => watch.ElapsedMilliseconds;
    public string Query { get; private set; }

    public LuceneIndexStorage(
        IOptions<PresentationOptions> presentationOptions,
        IOptions<IndexStorageOptions> storageOptions)
    {
        _presentationOptions = presentationOptions.Value;
        _storageOptions = storageOptions.Value;
        _indexPath = Path.Combine(_storageOptions.Path, "index");
        analyzer = GetAnalyzer();
    }

    public static string Escape(string s)
    {
        return QueryParser.Escape(s);
    }

    public List<Book> Search(out int count, string query, string field = "", int page = 1)
    {
        var take = _presentationOptions.PageSize;
        var skip = (page - 1) * _presentationOptions.PageSize;
        var parser = new QueryParser(AppLuceneVersion, field, analyzer);
        var q = parser.Parse(query);
        return Search(q, take, skip, out count);
    }

    public List<Book> SearchExact(out int count, string field, string value, int take = 10, int skip = 0)
    {
        var q = new BooleanQuery();
        var phraseQuery = new PhraseQuery
        {
            new Term(field, value)
        };
        q.Add(phraseQuery, Occur.MUST);
        return Search(q, take, skip, out count);
    }

    public List<Book> SearchExact(out int count, string field, string value, int page = 1)
    {
        var take = _presentationOptions.PageSize;
        var skip = (page - 1) * _presentationOptions.PageSize;
        return SearchExact(out count, field, value, take, skip);
    }

    protected List<Book> Search(Query query, int take, int skip, out int count)
    {
        Query = query.ToString();
        watch = Stopwatch.StartNew();
        var fields = new SortField[]
        {
            new SortField("title_sort", SortFieldType.STRING, false),
            new SortField("pubdate", SortFieldType.STRING, true),
            SortField.FIELD_SCORE
        };
        var sort = new Sort(fields);

        using var directory = FSDirectory.Open(_indexPath);
        using var reader = DirectoryReader.Open(directory);
        var searcher = new IndexSearcher(reader);
        var docs = searcher.Search(query, null, skip + take, sort);
        count = docs.TotalHits;

        var books = new List<Book>();
        for (int i = skip; i < docs.TotalHits; i++)
        {
            if (i > (skip + take) - 1)
            {
                break;
            }

            var doc = searcher.Doc(docs.ScoreDocs[i].Doc);
            var authors = doc.GetFields("author_fullname")
                .Select(x => x.GetStringValue().Split(','))
                .Select(x => new Author { FirstName = x[0], MiddleName = x[1], LastName = x[2] });
            var genres = doc.GetFields("genre")
                .Select(x => x.GetStringValue())
                .Select(x => GenreExtensions.Construct(x));

            Cover? cover = null;
            var coverContentType = doc.Get("_cover_type");
            if (coverContentType != null)
            {
                cover = new Cover
                {
                    Data = doc.GetBinaryValue("_cover_data").Bytes,
                    ContentType = coverContentType
                };
            }

            var meta = new List<MetaField>();
            foreach (var f in doc.Fields)
            {
                if (!KNOWN_FIELDS.Contains(f.Name, StringComparer.OrdinalIgnoreCase))
                {
                    meta.Add(new MetaField { Name = f.Name, Value = f.GetStringValue() });
                }
            }

            var book = new Book
            {
                Id = Guid.Parse(doc.Get("guid")),
                LibraryId = Guid.Parse(doc.Get("_library_id")),
                UpdatedFromFile = bool.Parse(doc.Get("_updated_from_file")),
                UpdatedAt = DateTools.StringToDate(doc.Get("_updated_at")),
                Title = doc.Get("title"),
                Series = doc.Get("series"),
                SeriesNo = int.Parse(doc.Get("seriesno")),
                File = doc.Get("file"),
                Ext = doc.Get("ext"),
                Date = DateTools.StringToDate(doc.Get("pubdate")),
                Archive = doc.Get("archive"),
                Authors = authors,
                Genres = genres,
                Annotation = doc.Get("annotation"),
                Language = doc.Get("language"),
                Cover = cover,
                Meta = meta
            };
            books.Add(book);
        }

        watch.Stop();

        return books;
    }

    public void Insert(Book book)
    {
        // delete previous version
        var q = new Term("_unique_id", GetBookUniqueId(book));
        writer.DeleteDocuments(q);

        var document = MapBook(book);
        writer.AddDocument(document);
    }

    public void Replace(Book book)
    {
        writer.PrepareCommit();
        try
        {
            var document = MapBook(book);
            writer.DeleteDocuments(new Term("guid", book.Id.ToString()));
            writer.AddDocument(document);
        }
        catch (Exception)
        {
            writer.Rollback();
        }
        writer.Commit();
    }

    public void Dispose()
    {
        writer?.Dispose();
        analyzer?.Dispose();
        directory?.Dispose();
    }

    public void Open()
    {
        directory = FSDirectory.Open(_indexPath);
        var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
        writer = new IndexWriter(directory, indexConfig);
    }

    private static string GetBookUniqueId(Book book)
    {
        return book.LibraryId + book.File + book.Ext + book.Archive;
    }

    private static Document MapBook(Book book)
    {
        var titleSort = book.Title!.TrimStart().TrimStart('«');
        var document = new Document
        {
            new StringField("_updated_at", DateTools.DateToString(DateTime.UtcNow, DateTools.Resolution.SECOND), Field.Store.YES),
            new StoredField("_updated_from_file", book.UpdatedFromFile.ToString()),
            new StringField("_unique_id", GetBookUniqueId(book), Field.Store.NO),
            new StringField("guid", (book.Id != Guid.Empty ? book.Id : Guid.NewGuid()).ToString(), Field.Store.YES),
            new StringField("_library_id", book.LibraryId.ToString(), Field.Store.YES),
            new TextField("title", book.Title, Field.Store.YES),
            new StringField("title_sort", titleSort, Field.Store.NO),
            new TextField("series", book.Series ?? "", Field.Store.YES),
            new StringField("series_exact", book.Series ?? "", Field.Store.NO),
            new StoredField("seriesno", book.SeriesNo.ToString()),
            new StoredField("file", book.File),
            new StoredField("ext", book.Ext),
            new StringField("pubdate", DateTools.DateToString(book.Date, DateTools.Resolution.DAY), Field.Store.YES),
            new StoredField("archive", book.Archive ?? ""),
            new TextField("annotation", book.Annotation ?? "", Field.Store.YES),
            new StringField("language", book.Language ?? "", Field.Store.YES)
        };
        if (book.Cover != null && book.Cover.Data != null)
        {
            document.Add(new StoredField("_cover_type", book.Cover.ContentType));
            document.Add(new StoredField("_cover_data", book.Cover.Data, 0, book.Cover.Data.Length));
        }

        foreach (var author in book.Authors)
        {
            var fullName = author.GetScreenName();
            var fullNameStore = string.Format("{0},{1},{2}", author.FirstName, author.MiddleName, author.LastName);
            document.Add(new TextField("author", fullName, Field.Store.NO));
            document.Add(new StringField("author_exact", fullName, Field.Store.NO));
            document.Add(new StoredField("author_fullname", fullNameStore));
        }

        if (book.Genres != null)
        foreach (var genre in book.Genres)
        {
            document.Add(new StringField("genre", genre.GetFullName(), Field.Store.YES));
        }

        if (book.Meta != null)
        foreach (var meta in book.Meta)
        {
            Field field = meta.IsAnalyzed ? new TextField(meta.Name, meta.Value, Field.Store.YES) : new StringField(meta.Name, meta.Value, Field.Store.YES);
            document.Add(field);
        }

        return document;
    }

    public int RemoveLibrary(string id)
    {
        var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
        using var directory = FSDirectory.Open(_indexPath);
        using var writer = new IndexWriter(directory, indexConfig);
        var query = new TermQuery(new Term("_library_id", id));

        int total = 0;
        using (var reader = writer.GetReader(true))
        {
            var searcher = new IndexSearcher(reader);
            var docs = searcher.Search(query, 1);
            total = docs.TotalHits;
        }

        writer.DeleteDocuments(query);
        writer.Flush(true, true);

        return total;
    }

    public int CleanupLibrary(string libraryId, DateTime startedAt)
    {
        var query = new BooleanQuery
        {
            { new TermRangeQuery("_updated_at",
                new BytesRef("*"),
                new BytesRef(DateTools.DateToString(startedAt, DateTools.Resolution.SECOND)), true, false),
                Occur.MUST },
            { new TermQuery(new Term("_library_id", libraryId)), Occur.MUST },
        };

        writer.Commit();
        int total = 0;
        var searcher = new IndexSearcher(writer.GetReader(true));
        var docs = searcher.Search(query, 1);
        total = docs.TotalHits;

        writer.DeleteDocuments(query);
        writer.Flush(true, true);

        return total;
    }

    public IDictionary<string, Tuple<string, bool>> GetAllGenres(string startsWith)
    {
        const string field = "genre";
        using var directory = FSDirectory.Open(_indexPath);
        using var reader = DirectoryReader.Open(directory);
        var result = new SortedDictionary<string, Tuple<string, bool>>();
        var fields = MultiFields.GetFields(reader);
        var terms = fields.GetTerms(field);

        foreach (var term1 in terms)
        {
            var term = term1.Term;
            var text = term.Utf8ToString();
            if (text.StartsWith(startsWith))
            {
                var name = GenreExtensions.Cut(text, startsWith);
                var displayName = GenreExtensions.GetNthName(name, 0);
                var fullName = GenreExtensions.Combine(startsWith, displayName);
                var isLast = GenreExtensions.IsLast(name);

                result[displayName] = new Tuple<string, bool>(fullName, isLast);
            }
        }

        return result;
    }

    private Analyzer GetAnalyzer() => _presentationOptions.DefaultLanguage switch
    {
        "ru" => new RussianAnalyzer(AppLuceneVersion),
        "en" => new EnglishAnalyzer(AppLuceneVersion),
        _ => new StandardAnalyzer(AppLuceneVersion),
    };
}

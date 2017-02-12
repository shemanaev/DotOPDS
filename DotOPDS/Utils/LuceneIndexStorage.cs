using DotOPDS.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Utils
{
    class LuceneIndexStorage : IDisposable
    {
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

        private IndexWriter writer;
        private Analyzer analyzer;
        private SimpleFSDirectory directory;
        private Stopwatch watch = Stopwatch.StartNew();

        public long Time => watch.ElapsedMilliseconds;
        public string Query { get; private set; }

        public string Escape(string s)
        {
            return QueryParser.Escape(s);
        }

        public List<Book> Search(out int count, string query, string field = "", int page = 1)
        {
            var take = Settings.Instance.Pagination;
            var skip = (page - 1) * Settings.Instance.Pagination;
            using (var analyzer = GetAnalyzer())
            {
                var parser = new QueryParser(Version.LUCENE_30, field, analyzer);
                var q = parser.Parse(query);
                return Search(q, take, skip, out count);
            }
        }

        public List<Book> SearchExact(out int count, string field, string value, int take = 10, int skip = 0)
        {
            var q = new BooleanQuery();
            var phraseQuery = new PhraseQuery();
            phraseQuery.Add(new Term(field, value));
            q.Add(phraseQuery, Occur.MUST);
            return Search(q, take, skip, out count);
        }

        public List<Book> SearchExact(out int count, string field, string value, int page = 1)
        {
            var take = Settings.Instance.Pagination;
            var skip = (page - 1) * Settings.Instance.Pagination;
            return SearchExact(out count, field, value, take, skip);
        }

        protected List<Book> Search(Query query, int take, int skip, out int count)
        {
            Query = query.ToString();
            watch = Stopwatch.StartNew();
            var fields = new SortField[]
            {
                new SortField("title_sort", SortField.STRING, false),
                new SortField("pubdate", SortField.STRING, true),
                SortField.FIELD_SCORE
            };
            var sort = new Sort(fields);

            using (var directory = new SimpleFSDirectory(new DirectoryInfo(Settings.Instance.DatabaseIndex)))
            using (var searcher = new IndexSearcher(directory))
            {
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
                        .Select(x => x.StringValue.Split(','))
                        .Select(x => new Author { FirstName = x[0], MiddleName = x[1], LastName = x[2] });
                    var genres = doc.GetFields("genre")
                        .Select(x => x.StringValue)
                        .Select(x => GenreExtensions.Construct(x));

                    Cover cover = null;
                    var coverContentType = doc.Get("_cover_type");
                    if (coverContentType != null)
                    {
                        cover = new Cover
                        {
                            Data = doc.GetBinaryValue("_cover_data"),
                            ContentType = coverContentType
                        };
                    }

                    var meta = new List<MetaField>();
                    var docFields = doc.GetFields();
                    foreach (var f in docFields)
                    {
                        if (!KNOWN_FIELDS.Contains(f.Name, StringComparer.OrdinalIgnoreCase))
                        {
                            meta.Add(new MetaField { Name = f.Name, Value = f.StringValue });
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
                        Date = DateTime.Parse(doc.Get("pubdate")),
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

        public void Open(string connection)
        {
            analyzer = GetAnalyzer();
            directory = new SimpleFSDirectory(new System.IO.DirectoryInfo(connection));
            writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private string GetBookUniqueId(Book book)
        {
            return book.LibraryId + book.File + book.Ext + book.Archive;
        }

        private Document MapBook(Book book)
        {
            var titleSort = book.Title.TrimStart().TrimStart(new char[] { '«' });
            var document = new Document();
            document.Add(new Field("_updated_at", DateTools.DateToString(DateTime.UtcNow, DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            document.Add(new Field("_updated_from_file", book.UpdatedFromFile.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("_unique_id", GetBookUniqueId(book), Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
            document.Add(new Field("guid", (book.Id != Guid.Empty ? book.Id : Guid.NewGuid()).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("_library_id", book.LibraryId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("title_sort", titleSort, Field.Store.NO, Field.Index.NOT_ANALYZED));
            document.Add(new Field("series", book.Series ?? "", Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("series_exact", book.Series ?? "", Field.Store.NO, Field.Index.NOT_ANALYZED));
            document.Add(new Field("seriesno", book.SeriesNo.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("file", book.File, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("ext", book.Ext, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("pubdate", book.Date.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("archive", book.Archive ?? "", Field.Store.YES, Field.Index.NO));
            document.Add(new Field("annotation", book.Annotation ?? "", Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("language", book.Language ?? "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            if (book.Cover != null)
            {
                document.Add(new Field("_cover_type", book.Cover.ContentType, Field.Store.YES, Field.Index.NO));
                document.Add(new Field("_cover_data", book.Cover.Data, 0, book.Cover.Data.Length, Field.Store.YES));
            }

            foreach (var author in book.Authors)
            {
                var fullName = author.GetScreenName();
                var fullNameStore = string.Format("{0},{1},{2}", author.FirstName, author.MiddleName, author.LastName);
                document.Add(new Field("author", fullName, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("author_exact", fullName, Field.Store.NO, Field.Index.NOT_ANALYZED));
                document.Add(new Field("author_fullname", fullNameStore, Field.Store.YES, Field.Index.NO));
            }

            if (book.Genres != null)
            foreach (var genre in book.Genres)
            {
                document.Add(new Field("genre", genre.GetFullName(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            if (book.Meta != null)
            foreach (var meta in book.Meta)
            {
                document.Add(new Field(meta.Name, meta.Value, Field.Store.YES, meta.IsAnalyzed ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));
            }

            return document;
        }

        public int RemoveLibrary(string database, string id)
        {
            using (var analyzer = GetAnalyzer())
            using (var directory = new SimpleFSDirectory(new DirectoryInfo(database)))
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var query = new TermQuery(new Term("_library_id", id));

                int total = 0;
                using (var searcher = new IndexSearcher(directory))
                {
                    var docs = searcher.Search(query, 1);
                    total = docs.TotalHits;
                }

                writer.DeleteDocuments(query);
                writer.Optimize(true);

                return total;
            }
        }

        public int CleanupLibrary(string libraryId, DateTime startedAt)
        {
            var query = new BooleanQuery {
                { new TermRangeQuery("_updated_at", "*", DateTools.DateToString(startedAt, DateTools.Resolution.SECOND), true, false), Occur.MUST },
                { new TermQuery(new Term("_library_id", libraryId)), Occur.MUST },
            };

            writer.Commit();
            int total = 0;
            using (var searcher = new IndexSearcher(directory))
            {
                var docs = searcher.Search(query, 1);
                total = docs.TotalHits;
            }

            writer.DeleteDocuments(query);
            writer.Optimize(true);

            return total;
        }

        public IDictionary<string, Tuple<string, bool>> GetAllGenres(string startsWith)
        {
            const string field = "genre";
            using (var directory = new SimpleFSDirectory(new DirectoryInfo(Settings.Instance.DatabaseIndex)))
            using (var reader = IndexReader.Open(directory, true)) {
                var result = new SortedDictionary<string, Tuple<string, bool>>();
                var terms = reader.Terms(new Term(field));
               
                while (terms.Next())
                {
                    var term = terms.Term;
                    if (term.Field == field && term.Text.StartsWith(startsWith))
                    {
                        var name = GenreExtensions.Cut(term.Text, startsWith);
                        var displayName = GenreExtensions.GetNthName(name, 0);
                        var fullName = GenreExtensions.Combine(startsWith, displayName);
                        var isLast = GenreExtensions.IsLast(name);

                        result[displayName] = new Tuple<string, bool>(fullName, isLast);
                    }
                }

                return result;
            }
        }

        private Analyzer GetAnalyzer()
        {
            switch (Settings.Instance.Language)
            {
                case "ru":
                    return new RussianAnalyzer(Version.LUCENE_30);

                default:
                    return new StandardAnalyzer(Version.LUCENE_30);
            }
        }
    }
}

using DotOPDS.Models;
using Lucene.Net.Analysis.Ru;
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
        private IndexWriter writer;
        private RussianAnalyzer analyzer;
        private SimpleFSDirectory directory;
        private Stopwatch watch;

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
            using (var analyzer = new RussianAnalyzer(Version.LUCENE_30))
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
                new SortField("Title.Sort", SortField.STRING, false),
                new SortField("Date", SortField.STRING, true),
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
                    var authors = doc.GetFields("Author.FullName")
                        .Select(x => x.StringValue.Split(','))
                        .Select(x => new Author { FirstName = x[0], MiddleName = x[1], LastName = x[2] })
                        .ToArray();
                    var genres = doc.GetFields("Genre")
                        .Select(x => x.StringValue)
                        .ToArray();

                    Cover cover = new Cover();
                    bool hasCover = false;
                    var hasValue = doc.Get("Cover.Has");
                    if (hasValue != null)
                    {
                        bool.TryParse(hasValue, out hasCover);
                        cover = new Cover { Has = hasCover };
                        if (hasCover)
                        {
                            cover.Data = doc.GetBinaryValue("Cover.Data");
                            cover.ContentType = doc.Get("Cover.Type");
                        }
                    }

                    var book = new Book
                    {
                        Id = Guid.Parse(doc.Get("Guid")),
                        LibraryId = Guid.Parse(doc.Get("LibraryId")),
                        Title = doc.Get("Title"),
                        Series = doc.Get("Series"),
                        SeriesNo = int.Parse(doc.Get("SeriesNo")),
                        File = doc.Get("File"),
                        Size = int.Parse(doc.Get("Size")),
                        LibId = int.Parse(doc.Get("LibId")),
                        Del = bool.Parse(doc.Get("Del")),
                        Ext = doc.Get("Ext"),
                        Date = DateTime.Parse(doc.Get("Date")),
                        Archive = doc.Get("Archive"),
                        Authors = authors,
                        Genres = genres,
                        Annotation = doc.Get("Annotation"),
                        Cover = cover,
                    };
                    books.Add(book);
                }

                watch.Stop();

                return books;
            }
        }

        public void Insert(Book book)
        {
            var document = MapBook(book);
            writer.AddDocument(document);
        }

        public void Replace(Book book)
        {
            writer.PrepareCommit();
            try
            {
                var document = MapBook(book);
                writer.DeleteDocuments(new Term("Guid", book.Id.ToString()));
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
            analyzer = new RussianAnalyzer(Version.LUCENE_30);
            directory = new SimpleFSDirectory(new System.IO.DirectoryInfo(connection));
            writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        private Document MapBook(Book book)
        {
            var titleSort = book.Title.TrimStart().TrimStart(new char[] { '«' });
            var document = new Document();
            document.Add(new Field("Guid", (book.Id != Guid.Empty ? book.Id : Guid.NewGuid()).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("LibraryId", book.LibraryId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Title.Sort", titleSort, Field.Store.NO, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Series", book.Series ?? "", Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Series.Exact", book.Series ?? "", Field.Store.NO, Field.Index.NOT_ANALYZED));
            document.Add(new Field("SeriesNo", book.SeriesNo.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("File", book.File, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Size", book.Size.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("LibId", book.LibId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Del", book.Del.ToString(), Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Ext", book.Ext, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Date", book.Date.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("Archive", book.Archive, Field.Store.YES, Field.Index.NO));
            document.Add(new Field("Annotation", book.Annotation ?? "", Field.Store.YES, Field.Index.NO));
            if (book.Cover?.Has != null)
            {
                document.Add(new Field("Cover.Has", book.Cover.Has.ToString(), Field.Store.YES, Field.Index.NO));
                if (book.Cover.Has == true)
                {
                    document.Add(new Field("Cover.Type", book.Cover.ContentType, Field.Store.YES, Field.Index.NO));
                    document.Add(new Field("Cover.Data", book.Cover.Data, 0, book.Cover.Data.Length, Field.Store.YES));
                }
            }


            foreach (var author in book.Authors)
            {
                var fullName = author.GetScreenName();
                var fullNameStore = string.Format("{0},{1},{2}", author.FirstName, author.MiddleName, author.LastName);
                var searchName = author.LastName ?? author.FirstName ?? author.MiddleName ?? "";
                document.Add(new Field("Author", fullName, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("Author.Exact", fullName, Field.Store.NO, Field.Index.NOT_ANALYZED));
                document.Add(new Field("Author.FullName", fullNameStore, Field.Store.YES, Field.Index.NO));
                document.Add(new Field("Author.SearchName", searchName, Field.Store.NO, Field.Index.NOT_ANALYZED));
            }

            foreach (var genre in book.Genres)
            {
                document.Add(new Field("Genre", genre, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            return document;
        }

        public int RemoveLibrary(string database, string id)
        {
            using (var analyzer = new RussianAnalyzer(Version.LUCENE_30))
            using (var directory = new SimpleFSDirectory(new DirectoryInfo(database)))
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var query = new TermQuery(new Term("LibraryId", id.ToString()));

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
    }
}

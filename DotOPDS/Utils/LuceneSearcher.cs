using DotOPDS.Models;
using Lucene.Net.Analysis.Ru;
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
    class LuceneSearcher
    {
        private Stopwatch watch;

        public long Time { get { return watch.ElapsedMilliseconds; } }
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
            var fields = new SortField[] { SortField.FIELD_SCORE, new SortField("LibId", SortField.INT, false) };
            var sort = new Sort(fields);

            using (var directory = new SimpleFSDirectory(new DirectoryInfo(Util.Normalize(Settings.Instance.Database))))
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
    }
}

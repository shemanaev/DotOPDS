using DotOPDS.Importers;
using DotOPDS.Utils;
using Lucene.Net.Analysis.Ru;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Nancy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Web.Controllers
{
    public class BaseRoute : NancyModule
    {
        private Stopwatch watchResponse;

        public BaseRoute()
        {
            Before += async (ctx, ct) =>
                {
                    await Task.Delay(0); // FIXME
                    watchResponse = Stopwatch.StartNew();
                    
                    int page;
                    if (!int.TryParse(Request.Query["page"], out page)) page = 1;
                    ctx.ViewBag.CurrentPage = page;

                    return null;
                };

            After += async (ctx, ct) =>
                {
                    await Task.Delay(0); // FIXME
                    watchResponse.Stop();
                    ctx.Response.Headers["X-Response-Time"] = string.Format("{0}ms", watchResponse.ElapsedMilliseconds);
                };
        }

        protected QueryParser GetQueryParser(string f)
        {
            var analyzer = new RussianAnalyzer(Version.LUCENE_30);
            return new QueryParser(Version.LUCENE_30, f, analyzer);
        }

        protected List<MetaBook> Search(Query query,/* int skip, int take,*/ out int count)
        {
            int skip = Settings.Instance.Pagination * (ViewBag.CurrentPage - 1);
            int take = Settings.Instance.Pagination;
            var watch = Stopwatch.StartNew();

            using (var directory = new SimpleFSDirectory(new DirectoryInfo(Settings.Instance.Database)))
            using (var searcher = new IndexSearcher(directory))
            {
                /*var query = new BooleanQuery();

                using (var analyzer = new RussianAnalyzer(Version.LUCENE_30))
                {
                    var parser = new QueryParser(Version.LUCENE_30, "Title", analyzer);

                    var keywordsQuery = parser.Parse(keywords);*/
                    //var termQuery = new TermQuery(new Term("Name", keywords));

                    //var phraseQuery = new PhraseQuery();
                    //phraseQuery.Add(new Term("Name", keywords));
                    //phraseQuery.Add(new Term("Name", "guitar"));
                    /*var phraseQuery = new MultiPhraseQuery();
                    string[] words = keywords.Split(' ');
                    var w = new List<Term>();
                    foreach (var word in words)
                    {
                        w.Add(new Term("Name", word));
                        //phraseQuery.Add(new Term("Name", word));
                    }
                    phraseQuery.Add(w.ToArray());*/
                 //   query.Add(keywordsQuery, Occur.MUST);
                    //query.Add(termQuery, Occur.MUST);
                    //query.Add(phraseQuery, Occur.SHOULD);
                    //Console.WriteLine("query: {0}", query);


                    //return query; // +Name:ibanez -Brand:Fender Name:"electric guitar"
                //}

                var docs = searcher.Search(query, null, skip + take);
                count = docs.TotalHits;

                var books = new List<MetaBook>();
                //foreach (var scoreDoc in docs.ScoreDocs)
                //{
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
                    var meta = new MetaBook
                    {
                        Id = Guid.Parse(doc.Get("Guid")),
                        LibraryId = Guid.Parse(doc.Get("LibraryId")),
                        Book = new Book
                        {
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
                        }
                    };
                    books.Add(meta);
                }

                watch.Stop();
                ViewBag.LuceneQuery = query.ToString();
                ViewBag.LuceneQueryTime = string.Format("{0}ms", watch.ElapsedMilliseconds);

                return books;
            }
        }
    }
}
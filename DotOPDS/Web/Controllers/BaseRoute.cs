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
using System.Threading.Tasks;
using Version = Lucene.Net.Util.Version;

namespace DotOPDS.Web.Controllers
{
    public class BaseRoute : NancyModule
    {
        private Stopwatch watchRequest;
        private Stopwatch watchQuery;

        public BaseRoute()
        {
            Before += async (ctx, ct) =>
                {
                    await Task.Delay(0); // FIXME
                    watchRequest = Stopwatch.StartNew();
                    return null;
                };

            After += async (ctx, ct) =>
                {
                    await Task.Delay(0); // FIXME
                    watchRequest.Stop();

                    ctx.Response.Headers["x-request-time"] = string.Format("{0}ms", watchRequest.ElapsedMilliseconds);
                    if (watchQuery != null)
                    {
                        ctx.Response.Headers["x-query-time"] = string.Format("{0}ms", watchQuery.ElapsedMilliseconds);
                    }
                };
        }

        protected QueryParser GetQueryParser(string f)
        {
            var analyzer = new RussianAnalyzer(Version.LUCENE_30);
            return new QueryParser(Version.LUCENE_30, f, analyzer);
        }

        protected List<MetaBook> Search(Query query, int limit, out int count)
        {
            Log.Debug("Lucene query: {0}", query);

            watchQuery = Stopwatch.StartNew();
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

                var docs = searcher.Search(query, null, limit);
                count = docs.TotalHits;

                var books = new List<MetaBook>();
                foreach (var scoreDoc in docs.ScoreDocs)
                {
                    var doc = searcher.Doc(scoreDoc.Doc);
                    var meta = new MetaBook
                    {
                        Id = Guid.Parse(doc.Get("Guid")),
                        LibraryId = Guid.Parse(doc.Get("LibraryId")),
                        Book = new Book
                        {
                            //Id = doc.Get("Id"), split Author.FullName by ','
                            Title = doc.Get("Title"),
                            Archive = doc.Get("Archive"),
                            File = doc.Get("File"),
                            Ext = doc.Get("Ext"),
                        }
                    };
                    books.Add(meta);
                }

                watchQuery.Stop();
                return books;
            }
        }
    }
}
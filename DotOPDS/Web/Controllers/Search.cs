using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Nancy;
using System;

namespace DotOPDS.Web.Controllers
{
    // http://www.opensearch.org/Specifications/OpenSearch/1.1
    public class Search : BaseRoute
    {
        public Search()
        {
            Get["/search"] = _ =>
            {
                dynamic model = null;
                var q = Request.Query.q;

                // http://stackoverflow.com/questions/5527868/exact-phrase-search-using-lucene
                // http://stackoverflow.com/questions/351176/paging-lucenes-search-results
                // http://stackoverflow.com/questions/963781/how-to-achieve-pagination-in-lucene
                // http://stackoverflow.com/questions/11451021/paging-lucene-net-search-results-asp-net

                if (q != null)
                {
                    q = (string)q;
                    // for non-advanced searches we should escape query
                    var simpleQuery = string.Format(@"Title:""{0}"" OR Author:""{0}"" OR Series:""{0}""", QueryParser.Escape(q.Trim()));
                    //var query = new BooleanQuery();
                    //query.Add(GetQueryParser("Title").Parse(simpleQuery), Occur.MUST);

                    int total;
                    // FIXME: add pagination
                    // TODO: use spellchecker if no results?
                    //var pagination = Settings.Instance.Pagination;
                    var query = GetQueryParser("Title").Parse(simpleQuery);
                    var books = Search(query, /*pagination * (ViewBag.CurrentPage - 1), pagination,*/ out total);
                    model = new { Books = books, Total = total, Query = q };
                    ViewBag.TotalPages = (int)Math.Ceiling((double)total / Settings.Instance.Pagination);
                }
                // http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx/
                // http://www.codeproject.com/Articles/639695/Getting-Started-with-Razor-View-Engine-in-MVC
                return View["Search", model];
            };

            Get[@"/genre/{genre}"] = x =>
            {
                dynamic model = null;

                var query = new BooleanQuery();
                var phraseQuery = new PhraseQuery();
                phraseQuery.Add(new Term("Genre", x.genre));
                query.Add(phraseQuery, Occur.MUST);

                int total;
                //var pagination = Settings.Instance.Pagination;
                var books = Search(query, /*pagination * (ViewBag.CurrentPage - 1), pagination,*/ out total);
                model = new { Books = books, Total = total, Genre = x.genre };
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / Settings.Instance.Pagination);

                return View["Genre", model];
            };

            Get[@"/author/{author}"] = x =>
            {
                dynamic model = null;

                //var simpleQuery = string.Format(@"+Author:""{0}""", QueryParser.Escape(x.author));
                //var query = GetQueryParser("Author").Parse(simpleQuery);
                var query = new BooleanQuery();
                var phraseQuery = new PhraseQuery();
                phraseQuery.Add(new Term("Author.Exact", x.author));
                query.Add(phraseQuery, Occur.MUST);

                int total;
                //var pagination = Settings.Instance.Pagination;
                var books = Search(query, /*pagination * (ViewBag.CurrentPage - 1), pagination,*/ out total);
                model = new { Books = books, Total = total, Author = x.author };
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / Settings.Instance.Pagination);

                return View["Author", model];
            };

            Get[@"/series/{series}"] = x =>
            {
                dynamic model = null;

                //var simpleQuery = string.Format(@"+Series:""{0}""", QueryParser.Escape(x.series));
                //var query = GetQueryParser("Series").Parse(simpleQuery);

                var query = new BooleanQuery();
                var phraseQuery = new PhraseQuery();
                phraseQuery.Add(new Term("Series.Exact", x.series));
                query.Add(phraseQuery, Occur.MUST);

                int total;
                //var pagination = Settings.Instance.Pagination;
                var books = Search(query, /*pagination * (ViewBag.CurrentPage - 1), pagination,*/ out total);
                model = new { Books = books, Total = total, Series = x.series };
                ViewBag.TotalPages = (int)Math.Ceiling((double)total / Settings.Instance.Pagination);

                return View["Series", model];
            };
        }
    }
}

using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Nancy;

namespace DotOPDS.Web.Controllers
{
    // http://www.opensearch.org/Specifications/OpenSearch/1.1
    public class Search : BaseRoute
    {
        public Search()
        {
            Get["/search"] = x =>
            {
                dynamic books = null;
                var q = Request.Query["q"];

                // http://stackoverflow.com/questions/5527868/exact-phrase-search-using-lucene
                // http://stackoverflow.com/questions/351176/paging-lucenes-search-results
                // http://stackoverflow.com/questions/963781/how-to-achieve-pagination-in-lucene
                // http://stackoverflow.com/questions/11451021/paging-lucene-net-search-results-asp-net

                if (q != null)
                {
                    // for non-advanced searches we should escape query
                    var simpleQuery = string.Format(@"Title:""{0}"" OR Author:""{0}"" OR Series:""{0}""", QueryParser.Escape(q));
                    var query = new BooleanQuery();
                    query.Add(GetQueryParser("Title").Parse(simpleQuery), Occur.MUST);

                    int total;
                    // FIXME: add pagination
                    // TODO: use spellchecker if no results?
                    books = Search(query, 10, out total);
                }
                // http://haacked.com/archive/2011/01/06/razor-syntax-quick-reference.aspx/
                // http://www.codeproject.com/Articles/639695/Getting-Started-with-Razor-View-Engine-in-MVC
                return View["Search", books];
            };
        }
    }
}

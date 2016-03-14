using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace DotOPDS.Web.Controllers
{
#if !DEBUG
    public
#endif
    class PageNotFound : DefaultViewRenderer, IStatusCodeHandler
    {
        public PageNotFound(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "NotFound");
            response.StatusCode = HttpStatusCode.NotFound;
            context.Response = response;
        }
    }
}

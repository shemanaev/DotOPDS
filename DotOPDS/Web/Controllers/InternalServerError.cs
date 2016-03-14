using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace DotOPDS.Web.Controllers
{
#if !DEBUG
    public
#endif
    class InternalServerError : DefaultViewRenderer, IStatusCodeHandler
    {
        public InternalServerError(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "InternalServerError");
            response.StatusCode = HttpStatusCode.NotFound;
            context.Response = response;
        }
    }
}

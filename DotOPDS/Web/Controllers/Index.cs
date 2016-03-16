namespace DotOPDS.Web.Controllers
{
    public class Index : BaseRoute
    {
        public Index()
        {
            Get["/"] = x =>
            {
                return View["Index"];
            };
        }
    }
}

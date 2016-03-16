using Nancy.ViewEngines.Razor;
using System.Collections.Generic;

namespace DotOPDS.Web
{
    public class RazorConfig : IRazorConfiguration
    {
        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "DotOPDS";
            yield return "DotOPDS.Assets";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "System.Linq";
            yield return "System.Collections.Generic";
            yield return "Nancy.ViewEngines.Razor";
            yield return "DotOPDS";
            yield return "DotOPDS.Utils";
            yield return "DotOPDS.Importers";
        }
    }
}

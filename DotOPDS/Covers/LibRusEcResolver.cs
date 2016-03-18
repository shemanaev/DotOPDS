using DotOPDS.Models;

namespace DotOPDS.Covers
{
    public class LibRusEcResolver : ICoverResolver
    {
        public string Name { get { return "librusec"; } }

        public string Get(Book book)
        {
            return string.Format("http://lib.rus.ec/cover/{0}", book.LibId);
        }
    }
}

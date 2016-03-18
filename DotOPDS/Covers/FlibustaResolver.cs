using System;
using DotOPDS.Models;

namespace DotOPDS.Covers
{
    public class FlibustaResolver : ICoverResolver
    {
        public string Name { get { return "flibusta"; } }

        public string Get(Book book)
        {
            throw new NotImplementedException("Nothing for now :(");
            /*
            var id = book.LibId.ToString();
            return string.Format("http://flibusta.is/i/{0}/{1}/_0.jpg", id[id.Length - 1], id);
            */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotOPDS.Utils
{
    public class MimeHelper
    {
        private static Dictionary<string, string> mime = new Dictionary<string, string>
        {
            {"jpeg", "image/jpeg"},
            {"fb2", "application/fb2"},
            {"fb2.zip", "application/fb2+zip"},
            {"epub", "application/epub+zip"},
        };

        public static string GetMimeType(string ext)
        {
            ext = ext.ToLowerInvariant();
            if (mime.ContainsKey(ext)) return mime[ext];
            return "application/octet-stream";
        }
    }
}

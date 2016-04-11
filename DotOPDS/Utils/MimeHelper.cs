using System.Collections.Generic;

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
            {"mobi", "application/x-mobipocket-ebook"},
            {"azw", "application/vnd.amazon.ebook"},
            {"azw3", "application/vnd.amazon.ebook"},
            {"djv", "image/x-djvu"},
            {"djvu", "image/x-djvu"},
            {"doc", "application/msword"},
            {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {"pdf", "application/pdf"},
            {"txt", "text/plain"},
            {"rtf", "text/rtf"},
        };

        public static string GetMimeType(string ext)
        {
            ext = ext.ToLowerInvariant();
            if (mime.ContainsKey(ext)) return mime[ext];
            return "application/octet-stream";
        }
    }
}

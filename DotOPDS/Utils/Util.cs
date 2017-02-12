using DotOPDS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotOPDS.Utils
{
    class Util
    {
        public static string Normalize(string path)
        {
            var from = IsLinux ? '\\' : '/';
            var to = IsLinux ? '/' : '\\';
            if (string.IsNullOrWhiteSpace(path)) return path;
            return Path.GetFullPath(Environment.ExpandEnvironmentVariables(path)).Replace(from, to);
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private static Dictionary<char, string> dangerChars = new Dictionary<char, string>
        {
            { '/', "" },
            { '\\', "" },
            { ':', "" },
            { '*', "" },
            { '?', "" },
            { '"', "'" },
            { '<', "«" },
            { '>', "»" },
            { '|', "" },
        };

        private static string FilterDangerChars(string s)
        {
            var res = "";
            foreach (var c in s)
            {
                if (dangerChars.ContainsKey(c)) res += dangerChars[c];
                else res += c;
            }
            return res;
        }

        public static string GetBookSafeName(Book book, string ext)
        {
            var result = book.Title;
            var firstAuthor = book.Authors.FirstOrDefault();
            if (firstAuthor != null)
            {
                result = string.Format("{0} - {1}", firstAuthor.GetScreenName(), result);
            }
            return string.Format("{0}.{1}", FilterDangerChars(result), ext);
        }
    }
}

using DotOPDS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DotOPDS.Utils
{
    class Util
    {
        public static string Normalize(string path)
        {
            var from = IsLinux ? '\\' : '/';
            var to = IsLinux ? '/' : '\\';
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

        private static Regex reEncoding = new Regex("encoding=\"(.+?)\"", RegexOptions.IgnoreCase);
        public static Encoding DetectXmlEncoding(Stream stream)
        {
            var result = Encoding.Default;
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                string line;
                do
                {
                    line = reader.ReadLine();
                }
                while (string.IsNullOrWhiteSpace(line) && !reader.EndOfStream);

                if (line.Contains("encoding"))
                {
                    var res = reEncoding.Match(line);
                    if (res.Success)
                    {
                        result = Encoding.GetEncoding(res.Groups[1].Value);
                    }
                }
            }
            catch (Exception)
            {
            }

            stream.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public static string GetInnerXml(XElement el)
        {
            var elems = el.IgnoreNamespace().Elements().Select(o => o.ToString());
            var innerXml = string.Join("", elems).Trim();
            return innerXml;
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
            if (book.Authors.Length > 0)
            {
                result = string.Format("{0} - {1}", book.Authors[0].GetScreenName(), result);
            }
            return string.Format("{0}.{1}", FilterDangerChars(result), ext);
        }
    }
}

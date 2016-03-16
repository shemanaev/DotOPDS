using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;

namespace DotOPDS.Utils
{
    public static class Helpers
    {
        private static Dictionary<char, string> translit = new Dictionary<char, string>
        {
            { 'а', "a" },
            { 'б', "b" },
            { 'в', "v" },
            { 'г', "g" },
            { 'д', "d" },
            { 'е', "e" },
            { 'ё', "yo" },
            { 'ж', "zh" },
            { 'з', "z" },
            { 'и', "i" },
            { 'й', "y" },
            { 'к', "k" },
            { 'л', "l" },
            { 'м', "m" },
            { 'н', "n" },
            { 'о', "o" },
            { 'п', "p" },
            { 'р', "r" },
            { 'с', "s" },
            { 'т', "t" },
            { 'у', "u" },
            { 'ф', "f" },
            { 'х', "x" },
            { 'ц', "c" },
            { 'ч', "ch" },
            { 'ш', "sh" },
            { 'щ', "shh" },
            { 'ь', "q" },
            { 'ы', "wi" },
            { 'ъ', "j" },
            { 'э', "we" },
            { 'ю', "yu" },
            { 'я', "ya" },
            { ' ', "_" },
        };

        public static string EncodeFileName(string s)
        {
            var res = "";
            foreach (var c in s.ToLowerInvariant())
            {
                if (translit.ContainsKey(c)) res += translit[c];
                else res += c;
            }
            return res;
        }

        public static IHtmlString LoopWithSeparator(string separator, IEnumerable<object> items)
        {
            return new NonEncodedHtmlString(string.Join(separator, items));
        }

        private static string SearchQuery(string query, int page = 0)
        {
            var format = @" href=""{0}";
            if (page > 1)
            {
                if (query.Contains("?")) format += "&";
                else format += "?";
                format += "page={1}";
            }
            format += @"""";
            return string.Format(format, query, page);
        }

        public static IHtmlString Pager(int page, int total, int max, string query)
        {
            int sp = 1;
            string res = "";

            if (total < max) max = total;
            // Starting point
            if (page < max)
            {
                sp = 1;
            }
            else if (page >= (total - (int)Math.Floor((double)max / 2)))
            {
                sp = total - max + 1;
            }
            else if (page >= max)
            {
                sp = page - (int)Math.Floor((double)max / 2);
            }

            if (total > 1)
            {
                res += "<nav class=\"pagination-fix\">";
                res += "<ul class=\"pagination\">";
                res += "<li";
                if (page <= 1) res += " class=\"disabled\"";
                res += ">";
                res += "<a aria-label=\"Previous\"";
                if (page > 1) res += SearchQuery(query, page - 1);
                res += ">";

                res += "<span aria-hidden=\"true\">&laquo;</span>";
                res += "</a>";
                res += "</li>";

                for (var i = sp; i <= (sp + max - 1); i++)
                {
                    res += "<li";
                    if (page == i) res += " class=\"disabled\"";
                    res += ">";
                    res += "<a";
                    if (page != i) res += SearchQuery(query, i);
                    res += ">" + i + "</a>";
                    res += "</li>";
                }

                res += "<li";
                if (page + 1 > total) res += " class=\"disabled\"";
                res += ">";
                res += "<a aria-label=\"Next\"";
                if (page < total) res += SearchQuery(query, page + 1);
                res += ">";
                res += "<span aria-hidden=\"true\">&raquo;</span>";
                res += "</a>";
                res += "</li>";
                res += "</ul>";
                res += "</nav>";
            }

            return new NonEncodedHtmlString(res);
        }
    }
}

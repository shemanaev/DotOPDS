using DotOPDS.Models;
using System.Collections.Generic;

namespace DotOPDS.Utils
{
    public static class UrlNameEncoder
    {
        private static Dictionary<char, string> translit = new Dictionary<char, string>
        {
            { 'а', "a" },
            { 'б', "b" },
            { 'в', "v" },
            { 'г', "g" },
            { 'д', "d" },
            { 'е', "e" },
            { 'ё', "e" },
            { 'ж', "zh" },
            { 'з', "z" },
            { 'и', "i" },
            { 'й', "j" },
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
            { 'х', "h" },
            { 'ц', "c" },
            { 'ч', "ch" },
            { 'ш', "sh" },
            { 'щ', "shh" },
            { 'ь', "" },
            { 'ы', "y" },
            { 'ъ', "" },
            { 'э', "e" },
            { 'ю', "yu" },
            { 'я', "ya" },
            { ' ', "+" },
            { ':', "_" },
            { ',', "" },
            { '\'', "" },
            { '`', "" },
            { '«', "" },
            { '»', "" },
        };

        public static string Encode(string s)
        {
            var res = "";
            foreach (var c in s.ToLowerInvariant())
            {
                if (translit.ContainsKey(c)) res += translit[c];
                else res += c;
            }
            return res;
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

        public static string GetSafeName(Book book, string ext)
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

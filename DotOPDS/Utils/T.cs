using DotOPDS.Plugins;

namespace DotOPDS.Utils
{
    internal class T
    {
        private static ITranslator _Catalog = new Translator("DotOPDS", "en", "./locale");

        public static void ChangeLanguage(string name)
        {
            _Catalog = new Translator("DotOPDS", name, "./locale");
        }

        public static string _(string text)
        {
            return _Catalog._(text);
        }

        public static string _(string text, params object[] args)
        {
            return _Catalog._(text, args);
        }

        public static string _n(string text, string pluralText, long n)
        {
            return _Catalog._n(text, pluralText, n);
        }

        public static string _n(string text, string pluralText, long n, params object[] args)
        {
            return _Catalog._n(text, pluralText, n, args);
        }

        public static string _p(string context, string text)
        {
            return _Catalog._p(context, text);
        }

        public static string _p(string context, string text, params object[] args)
        {
            return _Catalog._p(context, text, args);
        }

        public static string _pn(string context, string text, string pluralText, long n)
        {
            return _Catalog._pn(context, text, pluralText, n);
        }

        public static string _pn(string context, string text, string pluralText, long n, params object[] args)
        {
            return _Catalog._pn(context, text, pluralText, n, args);
        }
    }
}

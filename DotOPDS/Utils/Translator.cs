using DotOPDS.Plugins;
using NGettext;
using System.Globalization;

namespace DotOPDS.Utils
{
    internal class Translator : ITranslator
    {
        private ICatalog catalog = new Catalog();

        public Translator(string name, string language, string directory = "./plugins/locale")
        {
            catalog = new Catalog(name, directory, new CultureInfo(language));
        }

        public string _(string text)
        {
            return catalog.GetString(text);
        }

        public string _(string text, params object[] args)
        {
            return catalog.GetString(text, args);
        }

        public string _n(string text, string pluralText, long n)
        {
            return catalog.GetPluralString(text, pluralText, n);
        }

        public string _n(string text, string pluralText, long n, params object[] args)
        {
            return catalog.GetPluralString(text, pluralText, n, args);
        }

        public string _p(string context, string text)
        {
            return catalog.GetParticularString(context, text);
        }

        public string _p(string context, string text, params object[] args)
        {
            return catalog.GetParticularString(context, text, args);
        }

        public string _pn(string context, string text, string pluralText, long n)
        {
            return catalog.GetParticularPluralString(context, text, pluralText, n);
        }

        public string _pn(string context, string text, string pluralText, long n, params object[] args)
        {
            return catalog.GetParticularPluralString(context, text, pluralText, n, args);
        }
    }
}

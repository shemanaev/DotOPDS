using DotOPDS.Contract;
using GetText;
using System.Globalization;

namespace DotOPDS.Shared.Plugins;

public class Translator : ITranslator
{
    private readonly ICatalog _catalog = new Catalog();

    public static ITranslator GetForRequest(CultureInfo culture)
    {
        return new Translator("DotOPDS", culture, "./locale");
    }

    public Translator(string name, string language, string directory = "./locale")
    {
        _catalog = new Catalog(name, directory, new CultureInfo(language));
    }

    public Translator(string name, CultureInfo culture, string directory = "./locale")
    {
        _catalog = new Catalog(name, directory, culture);
    }

    public string _(string text) =>
        _catalog.GetString(text);
    public string _(string text, params object[] args) =>
        _catalog.GetString(text, args);
    public string _n(string text, string pluralText, long n) =>
        _catalog.GetPluralString(text, pluralText, n);
    public string _n(string text, string pluralText, long n, params object[] args) =>
        _catalog.GetPluralString(text, pluralText, n, args);
    public string _p(string context, string text) =>
        _catalog.GetParticularString(context, text);
    public string _p(string context, string text, params object[] args) =>
        _catalog.GetParticularString(context, text, args);
    public string _pn(string context, string text, string pluralText, long n) =>
        _catalog.GetParticularPluralString(context, text, pluralText, n);
    public string _pn(string context, string text, string pluralText, long n, params object[] args) =>
        _catalog.GetParticularPluralString(context, text, pluralText, n, args);
}

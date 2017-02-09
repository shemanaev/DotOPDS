namespace DotOPDS.Plugins
{
    public interface ITranslator
    {
        string _(string text);
        string _(string text, params object[] args);
        string _n(string text, string pluralText, long n);
        string _n(string text, string pluralText, long n, params object[] args);
        string _p(string context, string text);
        string _p(string context, string text, params object[] args);
        string _pn(string context, string text, string pluralText, long n);
        string _pn(string context, string text, string pluralText, long n, params object[] args);
    }
}

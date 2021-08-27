using DotOPDS.Contract;

namespace DotOPDS.BookProvider.Inpx.Tests;

class MockTranslator : ITranslator
{
    public string _(string text) => text;

    public string _(string text, params object[] args) => text;

    public string _n(string text, string pluralText, long n) => text;

    public string _n(string text, string pluralText, long n, params object[] args) => text;

    public string _p(string context, string text) => text;

    public string _p(string context, string text, params object[] args) => text;

    public string _pn(string context, string text, string pluralText, long n) => text;

    public string _pn(string context, string text, string pluralText, long n, params object[] args) => text;
}

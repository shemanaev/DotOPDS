using DotOPDS.Models;

namespace DotOPDS.Covers
{
    interface ICoverResolver
    {
        string Name { get; }
        string Get(Book book);
    }
}

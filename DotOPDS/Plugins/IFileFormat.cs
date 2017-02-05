using DotOPDS.Models;
using System.IO;

namespace DotOPDS.Plugins
{
    public interface IFileFormat : IPlugin
    {
        string Extension { get; }
        bool Read(Book book, Stream file); // true if updated and we should store it, false otherwise
    }
}

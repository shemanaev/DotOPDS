using DotOPDS.Models;
using System.Collections.Generic;

namespace DotOPDS.Plugins
{
    public delegate void ImportBookCallback(Book book);
    public delegate void ImportFinishedCallback();

    public interface IBookProvider : IPlugin
    {
        string Command { get; }
        string Help { get; }
        IEnumerable<IndexField> IndexFields { get; }
        void Run(string library, string[] args, ImportBookCallback import, ImportFinishedCallback finished);
    }
}

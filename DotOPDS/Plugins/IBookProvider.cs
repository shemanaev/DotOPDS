using DotOPDS.Models;

namespace DotOPDS.Plugins
{
    public delegate void ImportBookCallback(Book book);
    public delegate void ImportFinishedCallback();

    public interface IBookProvider : IPlugin
    {
        string Command { get; }
        string Help { get; }
        void Run(string library, string[] args, ImportBookCallback import, ImportFinishedCallback finished);
    }
}

using DotOPDS.Models;

namespace DotOPDS.Plugins
{
    public delegate void ImportBookCallback(Book book);
    public delegate void ImportFinishedCallback();

    public interface IImporter : IPlugin
    {
        string Command { get; }
        string Help { get; }
        void Run(string[] args, ImportBookCallback import, ImportFinishedCallback finished);
    }
}

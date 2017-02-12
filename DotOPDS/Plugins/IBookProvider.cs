using DotOPDS.Models;
using System.Collections.Generic;

namespace DotOPDS.Plugins
{
    /// <summary>
    /// Called on every book.
    /// </summary>
    /// <param name="book">Info that should be indexed.</param>
    public delegate void ImportBookCallback(Book book);

    /// <summary>
    /// Called on import done.
    /// </summary>
    public delegate void ImportFinishedCallback();

    /// <summary>
    /// Books indexing provider.
    /// </summary>
    public interface IBookProvider : IPlugin
    {
        /// <summary>
        /// Name in command line.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Command line help. Displayed on "DotOPDS import help {<see cref="Command"/>}".
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Additional searchable fields indexed in <see cref="Models.Book.Meta"/>.
        /// </summary>
        IEnumerable<IndexField> IndexFields { get; }

        /// <summary>
        /// Main worker function.
        /// </summary>
        /// <param name="library">Path to actual book files folder.</param>
        /// <param name="args">Additional plugin related arguments.</param>
        /// <param name="import">Should be called on every book that needs to be indexed.</param>
        /// <param name="finished">Should be called when import done.</param>
        void Run(string library, string[] args, ImportBookCallback import, ImportFinishedCallback finished);
    }
}

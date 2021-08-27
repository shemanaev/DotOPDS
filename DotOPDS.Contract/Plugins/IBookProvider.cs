using DotOPDS.Contract.Models;
using System.Collections.Generic;

namespace DotOPDS.Contract.Plugins;

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
    /// Command line help. Displayed on "DotOPDS.Manage import help {<see cref="Command"/>}".
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
    IAsyncEnumerable<Book> GetBooksAsync(string library, string[] args);
}

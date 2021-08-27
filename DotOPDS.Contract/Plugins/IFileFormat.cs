using DotOPDS.Contract.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DotOPDS.Contract.Plugins;

/// <summary>
/// Lazy parsing and book info updating from file itself.
/// </summary>
public interface IFileFormat : IPlugin
{
    /// <summary>
    /// Supported extension.
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Update book info.
    /// </summary>
    /// <param name="book">Existing model from database.</param>
    /// <param name="file">File opened for reading.</param>
    /// <returns>True if book was updated and we should store it, false otherwise.</returns>
    Task<bool> ReadAsync(Book book, Stream file, CancellationToken cancellationToken);
}

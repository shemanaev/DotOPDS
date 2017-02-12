using DotOPDS.Models;
using System.IO;

namespace DotOPDS.Plugins
{
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
        bool Read(Book book, Stream file);
    }
}

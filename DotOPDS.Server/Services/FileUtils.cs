using DotOPDS.Contract.Models;
using DotOPDS.Shared;
using System.IO.Compression;

namespace DotOPDS.Services;

public class FileUtils
{
    private readonly ILogger<FileUtils> _logger;
    private readonly LibrariesIndex _libraries;

    public FileUtils(ILogger<FileUtils> logger, LibrariesIndex libraries)
    {
        _logger = logger;
        _libraries = libraries;
    }

    public Stream GetBookFile(Book book)
    {
        var libraryPath = _libraries.GetPathById(book.LibraryId);
        if (libraryPath == null || book.Archive == null)
        {
            throw new KeyNotFoundException("File not found.");
        }

        var archive = Path.Combine(libraryPath, book.Archive);
        var filename = string.Format("{0}.{1}", book.File, book.Ext);
        if (File.Exists(archive))
        {
            return GetBookFileFromArchive(archive, filename);
        }
            
        var fullpath = Path.Combine(libraryPath, filename);
        if (File.Exists(fullpath))
        {
            return File.OpenRead(fullpath);
        }

        throw new KeyNotFoundException("File not found.");
    }

    private Stream GetBookFileFromArchive(string archive, string filename)
    {
        _logger.LogTrace("Trying to open archive, {ArchiveName}", archive);
        var zip = ZipFile.OpenRead(archive);
        {
            _logger.LogDebug("Archive {ArchiveName} opened", archive);

            var entry = zip.GetEntry(filename);
            if (entry == null)
            {
                _logger.LogWarning("File {FileName} in archive {ArchiveName} not found.", filename, archive);
                throw new KeyNotFoundException("File not found.");
            }

            return entry.Open();
        }
    }
}

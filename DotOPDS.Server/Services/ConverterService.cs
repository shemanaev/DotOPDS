using System.Diagnostics;
using DotOPDS.Contract.Models;
using DotOPDS.Shared.Options;
using Microsoft.Extensions.Options;

namespace DotOPDS.Services;

public class ConverterService
{
    private readonly ILogger<ConverterService> _logger;
    private readonly PresentationOptions _options;
    private readonly FileUtils _fileUtils;

    public ConverterService(
        ILogger<ConverterService> logger,
        IOptions<PresentationOptions> options,
        FileUtils fileUtils)
    {
        _logger = logger;
        _options = options.Value;
        _fileUtils = fileUtils;
    }

    public IEnumerable<string> GetAvailableConvertersForExt(string extension) =>
        _options.Converters?
            .Where(x => x.From!.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.To!) ?? new List<string>();

    public async Task<Stream?> GetFileInFormatAsync(Book book, string extension, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Trying to convert file {FileName} from {FromExt} to {ToExt}", book.File, book.Ext, extension);

        var file = _fileUtils.GetBookFile(book);
        if (extension.Equals(book.Ext, StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogDebug("File {FileName}.{Ext} served directly from archive.", book.File, book.Ext);
            return file;
        }

        var converter = _options.Converters
            .FirstOrDefault(x => x.From.Equals(book.Ext, StringComparison.InvariantCultureIgnoreCase)
                              && x.To.Equals(extension, StringComparison.InvariantCultureIgnoreCase));

        if (converter == null)
        {
            _logger.LogWarning("No converter found for '{FromExt}'->'{ToExt}'", book.Ext, extension);
            return null;
        }

        var tempDir = Path.Combine(Path.GetTempPath(), "DotOPDS_Temp");
        Directory.CreateDirectory(tempDir);
        var from = Path.Combine(tempDir, $"{Path.GetRandomFileName()}{book.File}.{book.Ext}");
        var to = Path.Combine(tempDir, Path.GetRandomFileName()) + "." + converter.To;

        using (var output = File.Create(from))
        {
            await file.CopyToAsync(output, cancellationToken);
        }

        var args = string.Format(converter.Arguments!, from, to);
        _logger.LogDebug("Starting converter process: {Command} {Arguments}", converter.Command, args);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {

                FileName = converter.Command,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };
        process.Start();
        await process.WaitForExitAsync(cancellationToken);
        _logger.LogDebug("Converter process exited with code {ExitCode}", process.ExitCode);

        return File.OpenRead(to);
    }
}

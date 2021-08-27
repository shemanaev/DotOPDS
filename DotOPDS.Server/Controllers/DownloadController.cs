using DotOPDS.Contract.Models;
using DotOPDS.Extensions;
using DotOPDS.Shared;
using DotOPDS.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotOPDS.Controllers;

[Route("download")]
public class DownloadController : ControllerBase
{
    private readonly ILogger<DownloadController> _logger;
    private readonly LuceneIndexStorage _textSearch;
    private readonly FileUtils _fileUtils;
    private readonly MimeHelper _mimeHelper;
    private readonly ConverterService _converter;

    public DownloadController(
        ILogger<DownloadController> logger,
        LuceneIndexStorage textSearch,
        FileUtils fileUtils,
        MimeHelper mimeHelper,
        ConverterService converter)
    {
        _logger = logger;
        _textSearch = textSearch;
        _fileUtils = fileUtils;
        _mimeHelper = mimeHelper;
        _converter = converter;
    }

    [Route("file/{id:guid}/{ext}", Name = nameof(GetFile))]
    public async Task<IActionResult> GetFile(Guid id, string ext, CancellationToken cancellationToken)
    {
        var book = GetBookById(id);

        var content = await _converter.GetFileInFormatAsync(book, ext, cancellationToken);
        if (content == null)
        {
            return NotFound();
        }

        _logger.LogInformation("Book {Id} served to {ClientIp}", id, HttpContext.Connection.RemoteIpAddress?.ToString());
        return File(content, _mimeHelper.GetContentType(ext), GetBookSafeName(book, ext));
    }

    [Route("cover/{id:guid}", Name = nameof(GetCover))]
    public IActionResult GetCover(Guid id)
    {
        var book = GetBookById(id);

        if (book.Cover == null || book.Cover.Data == null || book.Cover.ContentType == null)
        {
            _logger.LogWarning("No cover found for file {Id}", id);
            return NotFound();
        }

        return File(book.Cover.Data, book.Cover.ContentType);
    }

    private Book GetBookById(Guid id)
    {
        var books = _textSearch.SearchExact(out int total, "guid", id.ToString(), take: 1);
        if (total != 1)
        {
            _logger.LogDebug("File {Id} not found", id);
            throw new KeyNotFoundException("Key Not Found: " + id);
        }

        _logger.LogDebug("File {Id} found in {Time}ms", id, _textSearch.Time);

        return books.First();
    }

    private static readonly Dictionary<char, string> _dangerChars = new()
    {
        { '/', "" },
        { '\\', "" },
        { ':', "" },
        { '*', "" },
        { '?', "" },
        { '"', "'" },
        { '<', "«" },
        { '>', "»" },
        { '|', "" },
    };

    private static string FilterDangerChars(string s)
    {
        var res = "";
        foreach (var c in s)
        {
            if (_dangerChars.ContainsKey(c)) res += _dangerChars[c];
            else res += c;
        }
        return res;
    }

    private static string GetBookSafeName(Book book, string ext)
    {
        var result = book.Title!;
        if (book.Authors != null)
        {
            var firstAuthor = book.Authors.FirstOrDefault();
            if (firstAuthor != default)
            {
                result = $"{firstAuthor.GetScreenName()} - {result}";
            }
        }
        return $"{FilterDangerChars(result)}.{ext}";
    }
}

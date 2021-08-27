using Microsoft.AspNetCore.StaticFiles;

namespace DotOPDS.Services;

public class MimeHelper
{
    private readonly FileExtensionContentTypeProvider _provider;

    public MimeHelper()
    {
        _provider = new FileExtensionContentTypeProvider();

        _provider.Mappings.Add(".fb2", "application/fb2");
        _provider.Mappings.Add(".fb2.zip", "application/fb2+zip");
        _provider.Mappings.Add(".epub", "application/epub+zip");
        _provider.Mappings.Add(".mobi", "application/x-mobipocket-ebook");
        _provider.Mappings.Add(".azw", "application/vnd.amazon.ebook");
        _provider.Mappings.Add(".azw3", "application/vnd.amazon.ebook");
        _provider.Mappings.Add(".djv", "image/x-djvu");
        _provider.Mappings.Add(".djvu", "image/x-djvu");
    }

    public string GetContentType(string path)
    {
        const string DefaultContentType = "application/octet-stream";
        var ext = "." + path.ToLowerInvariant();

        if (!_provider.TryGetContentType(ext, out var contentType))
        {
            contentType = DefaultContentType;
        }

        return contentType;
    }
}

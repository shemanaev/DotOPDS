using DotOPDS.Contract.Models;
using System.Xml.Linq;
using DotOPDS.Contract;
using Microsoft.Extensions.Logging;
using DotOPDS.Contract.Plugins;

namespace DotOPDS.FileFormat.Fb2;

public class Fb2 : IFileFormat
{
    public Version Version => new(1, 0);
    public string Name => "FB2 file format";
    public string Extension => "fb2";

    private readonly ILogger<Fb2> _logger;

    public Fb2(IHostedServices hostedServices)
    {
        _logger = hostedServices.GetLogger<Fb2>();
    }

    public async Task<bool> ReadAsync(Book book, Stream stream, CancellationToken cancellationToken)
    {
        var mem = new MemoryStream();
        stream.CopyTo(mem);
        var encoding = Util.DetectXmlEncoding(mem);
        _logger.LogTrace("Book encoding detected, id:{Id}, enc:{Encoding}", book.Id, encoding);

        using (var reader = new StreamReader(mem, encoding))
        {
            using var sgmlReader = new Sgml.SgmlReader() 
            {
                InputStream = reader
            };
            var doc = await XDocument.LoadAsync(sgmlReader, LoadOptions.PreserveWhitespace, cancellationToken);
            //var doc = XDocument.Load(sgmlReader);
            _logger.LogTrace("Book file loaded, id:{Id}", book.Id);

            try
            {
                UpdateAnnotation(book, doc);
            }
            catch (Exception) {}

            try
            {
                UpdateCover(book, doc);
            }
            catch (Exception) {}
        }

        return true;
    }

    private static void UpdateAnnotation(Book book, XDocument doc)
    {
        var annotation = doc
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "annotation");
        if (annotation != null)
        {
            book.Annotation = Util.GetInnerXml(annotation);
        }
    }

    private void ExtractImage(Book book, XDocument doc, XElement el)
    {
        var coverId = el.Attributes()
            .First(x => x.Name.LocalName == "href")
            .Value[1..];
        var cover = doc.Descendants()
            .First(x => x.Name.LocalName == "binary"
                     && x.Attribute("id")?.Value == coverId);
        var ctype = cover.Attribute("content-type")?.Value;
        var bin = Convert.FromBase64String(cover.Value);
        book.Cover = new Cover
        {
            Data = bin,
            ContentType = ctype
        };
    }

    private void UpdateCover(Book book, XDocument doc)
    {
        var coverPage = doc
            .Descendants()
            .Where(x => x.Name.LocalName == "coverpage")
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "image");

        if (coverPage != null)
        {
            ExtractImage(book, doc, coverPage);
        }
        else
        {
            var firstImage = doc
                .Descendants()
                .Where(x => x.Name.LocalName == "body")
                .Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "image");
            if (firstImage != null)
            {
                ExtractImage(book, doc, firstImage);
            }
        }
    }
}

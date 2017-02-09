using DotOPDS.Plugins;
using System;
using DotOPDS.Models;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace DotOPDS.Plugin.FileFormat.Fb2
{
    public class Fb2 : IFileFormat
    {
        public Version Version => new Version(1, 0);
        public string Name => "FB2 file format";
        public string Extension => "fb2";

        private IPluginHost _host;
        private ILogger logger;

        public bool Initialize(IPluginHost host)
        {
            _host = host;
            logger = _host.GetLogger("Fb2");
            return true;
        }

        public void Terminate()
        {
        }

        public bool Read(Book book, Stream stream)
        {
            var mem = new MemoryStream();
            stream.CopyTo(mem);
            var encoding = Util.DetectXmlEncoding(mem);
            logger.Trace($"Book encoding detected, id:{book.Id}, enc:{encoding}");

            using (var reader = new StreamReader(mem, encoding))
            {
                using (var sgmlReader = new Sgml.SgmlReader())
                {
                    sgmlReader.InputStream = reader;
                    var doc = XDocument.Load(sgmlReader);
                    logger.Trace($"Book file loaded, id:{book.Id}");

                    try
                    {
                        UpdateAnnotation(book, doc);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        UpdateCover(book, doc);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return true;
        }

        private void UpdateAnnotation(Book book, XDocument doc)
        {
            var annotation = doc.Descendants()
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
                                       .Value.Substring(1);
            var cover = doc.Descendants()
                           .First(x => x.Name.LocalName == "binary"
                                    && x.Attribute("id").Value == coverId);
            var ctype = cover.Attribute("content-type").Value;
            var bin = Convert.FromBase64String(cover.Value);
            book.Cover = new Cover
            {
                Data = bin,
                ContentType = ctype
            };
        }

        private void UpdateCover(Book book, XDocument doc)
        {
            var coverPage = doc.Descendants()
                               .Where(x => x.Name.LocalName == "coverpage")
                               .Descendants()
                               .FirstOrDefault(x => x.Name.LocalName == "image");

            if (coverPage != null)
            {
                ExtractImage(book, doc, coverPage);
            }
            else
            {
                var firstImage = doc.Descendants()
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
}

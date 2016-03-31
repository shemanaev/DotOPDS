using System;
using System.Linq;
using DotOPDS.Models;
using System.IO;
using System.Xml.Linq;
using DotOPDS.Utils;

namespace DotOPDS.Parsers
{
    class Fb2Parser : IBookParser
    {
        private void UpdateAnnotation(Book book, XDocument doc)
        {
            var annotation = doc.Descendants()
                                .Where(x => x.Name.LocalName == "annotation")
                                .FirstOrDefault();
            if (annotation != null)
            {
                book.Annotation = Util.GetInnerXml(annotation);
            }
        }

        private void UpdateCover(Book book, XDocument doc)
        {
            var coverPage = doc.Descendants()
                               .Where(x => x.Name.LocalName == "coverpage")
                               .Descendants()
                               .Where(x => x.Name.LocalName == "image")
                               .FirstOrDefault();

            if (coverPage != null)
            {
                var coverId = coverPage.Attributes()
                                       .Where(x => x.Name.LocalName == "href")
                                       .First()
                                       .Value.Substring(1);
                var cover = doc.Descendants()
                               .Where(x => x.Name.LocalName == "binary" && x.Attribute("id").Value == coverId)
                               .First();
                var ctype = cover.Attribute("content-type").Value;
                var bin = Convert.FromBase64String(cover.Value);
                book.Cover = new Cover
                {
                    Data = bin,
                    ContentType = ctype,
                    Has = true
                };
            }
        }

        public void Update(Book book)
        {
            using (var stream = FileUtils.GetBookFile(book))
            {
                var mem = new MemoryStream();
                stream.CopyTo(mem);
                var encoding = Util.DetectXmlEncoding(mem);
                using (var reader = new StreamReader(mem, encoding))
                {
                    using (var sgmlReader = new Sgml.SgmlReader())
                    {
                        sgmlReader.InputStream = reader;
                        var doc = XDocument.Load(sgmlReader);

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
                            book.Cover = new Cover { Has = false };
                        }
                    }
                }
            }
        }
    }
}

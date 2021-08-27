using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DotOPDS.FileFormat.Fb2;

internal class Util
{
    private static readonly Regex _reEncoding = new("encoding=\"(.+?)\"", RegexOptions.IgnoreCase);

    public static Encoding DetectXmlEncoding(Stream stream)
    {
        var result = Encoding.Default;
        try
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            string line;
            do
            {
                line = reader.ReadLine()!;
            }
            while (string.IsNullOrWhiteSpace(line) && !reader.EndOfStream);

            if (line.Contains("encoding"))
            {
                var res = _reEncoding.Match(line);
                if (res.Success)
                {
                    result = Encoding.GetEncoding(res.Groups[1].Value);
                }
            }
        }
        catch (Exception)
        {
        }

        stream.Seek(0, SeekOrigin.Begin);
        return result;
    }

    public static string GetInnerXml(XElement el)
    {
        var elems = el.IgnoreNamespace().Elements().Select(o => o.ToString());
        var innerXml = string.Join("", elems).Trim();
        return innerXml;
    }
}

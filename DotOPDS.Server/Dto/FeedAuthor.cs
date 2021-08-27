using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedAuthor
{
    [XmlElement("name")]
    public string? Name { get; set; }
    [XmlElement("uri")]
    public string? Uri { get; set; }

    public FeedAuthor() { }

    public FeedAuthor(string name, string uri)
    {
        Name = name;
        Uri = uri;
    }
}

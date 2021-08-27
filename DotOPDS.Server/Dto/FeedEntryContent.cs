using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedEntryContent
{
    [XmlAttribute("type")]
    public string Type { get; set; } = "text";
    [XmlText]
    public string? Text { get; set; }
}

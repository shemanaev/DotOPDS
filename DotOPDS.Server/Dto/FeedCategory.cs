using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedCategory
{
    [XmlAttribute("term")]
    public string? Term { get; set; }
    [XmlAttribute("label")]
    public string? Label { get; set; }

    public FeedCategory() { }

    public FeedCategory(string term, string label)
    {
        Term = term;
        Label = label;
    }
}

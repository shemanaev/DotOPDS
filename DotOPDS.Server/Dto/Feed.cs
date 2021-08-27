using System.Xml.Serialization;

namespace DotOPDS.Dto;

[XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
public class Feed
{
    [XmlElement("updated")]
    public string Updated { get; set; } = DateTime.UtcNow.ToString("s");
    [XmlElement("id")]
    public string? Id { get; set; }
    [XmlElement("title")]
    public string? Title { get; set; }
    [XmlElement("icon")]
    public string? Icon { get; set; }
    [XmlElement("totalResults", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
    public int? Total { get; set; }
    [XmlElement("itemsPerPage", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
    public int ItemsPerPage { get; set; } = 10;
    [XmlElement("link")]
    public List<FeedLink> Links { get; set; } = new();
    [XmlElement("entry")]
    public List<FeedEntry> Entries { get; set; } = new();

    public bool ShouldSerializeTotal()
    {
        return Total != null;
    }

    public bool ShouldSerializeItemsPerPage()
    {
        return ShouldSerializeTotal();
    }
}

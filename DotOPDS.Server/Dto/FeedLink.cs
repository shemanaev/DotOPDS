using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedLink
{
    [JsonIgnore]
    [XmlIgnore]
    public FeedLinkRel Rel { get; set; } = FeedLinkRel.Null;
    [JsonIgnore]
    [XmlIgnore]
    public FeedLinkType Type { get; set; }
    public string? MimeType { get; set; }
    [XmlAttribute("href")]
    public string? Href { get; set; }
    [XmlAttribute("title")]
    public string? Title { get; set; }

    public FeedLink() { }

    public FeedLink(string href, FeedLinkRel rel)
    {
        Href = href;
        Rel = rel;
    }

    public FeedLink(string href, FeedLinkType type)
    {
        Href = href;
        Type = type;
    }

    public FeedLink(string href, FeedLinkRel rel, FeedLinkType type)
    {
        Href = href;
        Rel = rel;
        Type = type;
    }

    public FeedLink(string href, FeedLinkRel rel, FeedLinkType type, string title)
    {
        Href = href;
        Rel = rel;
        Type = type;
        Title = title;
    }

    public FeedLink(string mimeType, string href, FeedLinkRel rel, FeedLinkType type)
    {
        MimeType = mimeType;
        Href = href;
        Rel = rel;
        Type = type;
    }

    [JsonPropertyName("rel")]
    [XmlAttribute("rel")]
    public string? _Rel
    {
        get => Rel.ToFriendlyString();
        set => Rel = FeedLinkRel.Null;
    }

    [JsonPropertyName("type")]
    [XmlAttribute("type")]
    public string? _Type
    {
        get => Type != FeedLinkType.Mime ? Type.ToFriendlyString() : MimeType;
        set => Type = FeedLinkType.Atom;
    }
}

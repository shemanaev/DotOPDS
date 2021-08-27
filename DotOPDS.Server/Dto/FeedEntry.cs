using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedEntry
{
    [XmlElement("updated")]
    public string Updated { get; set; } = DateTime.UtcNow.ToString("s");
    [XmlElement("id")]
    public string? Id { get; set; }
    [XmlElement("title")]
    public string? Title { get; set; }
    [XmlElement("issued", Namespace = "http://purl.org/dc/terms/")]
    public int Issued { get; set; }
    [XmlElement("language", Namespace = "http://purl.org/dc/terms/")]
    public string? Language { get; set; }
    [XmlElement("content")]
    public FeedEntryContent? Content { get; set; }
    [XmlElement("series", Namespace = "urn:dotopds:v1.0")]
    public FeedEntrySeries? Series { get; set; }
    [XmlElement("link")]
    public List<FeedLink> Links { get; set; } = new();
    [XmlElement("author")]
    public List<FeedAuthor> Authors { get; set; } = new();
    [XmlElement("category")]
    public List<FeedCategory> Categories { get; set; } = new();

    public FeedEntry() { }

    public FeedEntry(string id, string title, string content)
    {
        Id = id;
        Title = title;
        Content = new FeedEntryContent { Text = content };
    }
}

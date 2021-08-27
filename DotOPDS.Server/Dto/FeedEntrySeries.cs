using System.Xml.Serialization;

namespace DotOPDS.Dto;

public class FeedEntrySeries
{
    [XmlAttribute("name")]
    public string? Name { get; set; }
    [XmlAttribute("number")]
    public int Number { get; set; }

    public FeedEntrySeries() { }

    public FeedEntrySeries(string name, int number)
    {
        Name = name;
        Number = number;
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DotOPDS.Models
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Feed
    {
        [XmlElement("updated")]
        public string Updated = DateTime.UtcNow.ToString("s");
        [XmlElement("id")]
        public string Id;
        [XmlElement("title")]
        public string Title;
        [XmlElement("icon")]
        public string Icon;
        [XmlElement("totalResults", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public int? Total;
        [XmlElement("itemsPerPage", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public int ItemsPerPage = Settings.Instance.Pagination;
        [XmlElement("link")]
        public List<FeedLink> Links = new List<FeedLink>();
        [XmlElement("entry")]
        public List<FeedEntry> Entries = new List<FeedEntry>();

        public bool ShouldSerializeTotal()
        {
            return Total != null;
        }

        public bool ShouldSerializeItemsPerPage()
        {
            return ShouldSerializeTotal();
        }
    }

    public class FeedEntry
    {
        [XmlElement("updated")]
        public string Updated = DateTime.UtcNow.ToString("s");
        [XmlElement("id")]
        public string Id;
        [XmlElement("title")]
        public string Title;
        [XmlElement("issued", Namespace = "http://purl.org/dc/terms/")]
        public int Issued;
        [XmlElement("language", Namespace = "http://purl.org/dc/terms/")]
        public string Language;
        [XmlElement("content")]
        public FeedEntryContent Content;
        [XmlElement("series", Namespace = "urn:dotopds:v1.0")]
        public FeedEntrySeries Series;
        [XmlElement("link")]
        public List<FeedLink> Links = new List<FeedLink>();
        [XmlElement("author")]
        public List<FeedAuthor> Authors = new List<FeedAuthor>();
        [XmlElement("category")]
        public List<FeedCategory> Categories = new List<FeedCategory>();
    }

    public class FeedEntrySeries
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("number")]
        public int Number;
    }

    public class FeedEntryContent
    {
        [XmlAttribute("type")]
        public string Type = "text";
        [XmlText]
        public string Text;
    }

    public class FeedAuthor
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("uri")]
        public string Uri;
    }

    public class FeedCategory
    {
        [XmlAttribute("term")]
        public string Term;
        [XmlAttribute("label")]
        public string Label;
    }

    public class FeedLink
    {
        [XmlAttribute("rel")]
        public string Rel;
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("href")]
        public string Href;
        [XmlAttribute("title")]
        public string Title;
    }

    public static class FeedLinkType
    {
        public const string Atom = "application/atom+xml";
        public const string AtomSearch = "application/opensearchdescription+xml";
        public const string AtomNavigation = "application/atom+xml;profile=opds-catalog;kind=navigation";
        public const string AtomAcquisition = "application/atom+xml;profile=opds-catalog;kind=acquisition";
    }

    public static class FeedLinkRel
    {
        public const string Debug = "debug";
        public const string Search = "search";
        public const string Self = "self";
        public const string Start = "start";
        public const string Next = "next";
        public const string Prev = "prev";
        public const string Alternate = "alternate";
        public const string Related = "related";
        public const string Image = "http://opds-spec.org/image";
        public const string Thumbnail = "http://opds-spec.org/image/thumbnail";
        public const string Acquisition = "http://opds-spec.org/acquisition/open-access";
    }
}

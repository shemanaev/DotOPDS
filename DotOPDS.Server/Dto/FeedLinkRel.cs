namespace DotOPDS.Dto;

public enum FeedLinkRel
{
    Debug,
    Search,
    Self,
    Start,
    Next,
    Prev,
    Alternate,
    Related,
    Image,
    Thumbnail,
    Acquisition,
    Null,
}

public static class FeedLinkRelExtensions
{
    public static string? ToFriendlyString(this FeedLinkRel rel) =>
        rel switch
        {
            FeedLinkRel.Debug => "debug",
            FeedLinkRel.Search => "search",
            FeedLinkRel.Self => "self",
            FeedLinkRel.Start => "start",
            FeedLinkRel.Next => "next",
            FeedLinkRel.Prev => "prev",
            FeedLinkRel.Alternate => "alternate",
            FeedLinkRel.Related => "related",
            FeedLinkRel.Image => "http://opds-spec.org/image",
            FeedLinkRel.Thumbnail => "http://opds-spec.org/image/thumbnail",
            FeedLinkRel.Acquisition => "http://opds-spec.org/acquisition/open-access",
            FeedLinkRel.Null => null,
            _ => throw new System.NotImplementedException(),
        };
}

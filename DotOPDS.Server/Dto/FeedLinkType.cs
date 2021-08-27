namespace DotOPDS.Dto;

public enum FeedLinkType
{
    Atom,
    AtomSearch,
    AtomNavigation,
    AtomAcquisition,
    Mime,
#if DEBUG
    DebugQuery,
    DebugTime,
#endif
}

public static class FeedLinkTypeExtensions
{
    public static string ToFriendlyString(this FeedLinkType type) =>
        type switch
        {
            FeedLinkType.Atom => "application/atom+xml",
            FeedLinkType.AtomSearch => "application/opensearchdescription+xml",
            FeedLinkType.AtomNavigation => "application/atom+xml;profile=opds-catalog;kind=navigation",
            FeedLinkType.AtomAcquisition => "application/atom+xml;profile=opds-catalog;kind=acquisition",
            FeedLinkType.Mime => "application/octet-stream",
#if DEBUG
            FeedLinkType.DebugQuery => "query",
            FeedLinkType.DebugTime => "time",
#endif
            _ => throw new System.NotImplementedException(),
        };
}

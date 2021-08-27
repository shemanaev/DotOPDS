using System.Collections.Generic;

namespace DotOPDS.Shared.Options;

public class PresentationOptions
{
    public const string ConfigurationKey = "Presentation";

    public string? DefaultLanguage { get; set; }
    public int PageSize { get; set; }
    public string? Title { get; set; }
    public bool LazyInfoExtract { get; set; }
    public List<ConvertersOptions>? Converters { get; set; } = new();
}

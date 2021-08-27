using System;
using System.Collections.Generic;

namespace DotOPDS.Shared;

internal class LibrariesIndexFile
{
    public int Version { get; set; } = -1;
    public Dictionary<Guid, LibraryItem> Libraries { get; set; } = new();
}

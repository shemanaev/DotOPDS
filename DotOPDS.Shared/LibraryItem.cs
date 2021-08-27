namespace DotOPDS.Shared;

public class LibraryItem
{
    public string? Path { get; set; }

    public LibraryItem(string path)
    {
        Path = path;
    }
}

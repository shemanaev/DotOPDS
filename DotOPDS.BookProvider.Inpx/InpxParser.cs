using DotOPDS.Contract.Models;
using System.IO.Compression;
using System.Text;

namespace DotOPDS.BookProvider.Inpx;

// http://forum.home-lib.net/index.php?showtopic=18
// http://forum.home-lib.net/index.php?showtopic=16
// http://forum.home-lib.net/index.php?showtopic=329&#entry2655
// https://dotnetzip.codeplex.com/wikipage?title=CS-Examples&referringTitle=Examples
public class InpxParser : IDisposable
{
    private static readonly int[] _fb2ids = { 0, 65536 };
    private const string DefaultStructure = "AUTHOR;GENRE;TITLE;SERIES;SERNO;FILE;SIZE;LIBID;DEL;EXT;DATE;";
    private readonly Encoding _cp1251;

    private bool _disposed;
    private ZipArchive? _zip;
    private readonly Genres _genres;

    public bool IsFb2 { get; private set; }
    public string? Version { get; private set; }
    public string? Name { get; private set; }
    public string? FileName { get; private set; }
    public string? Description { get; private set; }

    public static async Task<InpxParser> Open(string fileName, Genres genres)
    {
        var result = new InpxParser(genres);
        await result.Create(fileName);
        return result;
    }

    private InpxParser(Genres genres)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _cp1251 = Encoding.GetEncoding(1251);

        _genres = genres;
    }

    private async Task Create(string filename)
    {
        const string collectionFile = "collection.info";
        _zip = ZipFile.Open(filename, ZipArchiveMode.Read, _cp1251);
        if (!_zip.Entries.Any(x => x.Name == collectionFile))
        {
            throw new FileNotFoundException("File not found in archive", collectionFile);
        }
        using (var reader = new StreamReader(_zip.GetEntry(collectionFile)!.Open(), _cp1251))
        {
            var comment = (await reader.ReadToEndAsync()).Split('\n');
            IsFb2 = _fb2ids.Contains(int.Parse(comment[2]));
            Name = comment[0];
            FileName = comment[1];
            Description = string.Join("\n", comment, 3, comment.Length - 3);
        }
        var result = _zip.Entries.First(entry => entry.Name.EndsWith("version.info"));
        if (result != null)
        {
            using var reader = new StreamReader(result.Open(), _cp1251);
            Version = (await reader.ReadToEndAsync()).Trim();
        }
    }

    public async IAsyncEnumerable<Book> GetBooksAsync()
    {
        var structure = await GetStructure();

        var inps = _zip!.Entries.Where(entry => entry.Name.EndsWith(".inp"));
        foreach (var inp in inps)
        {
            using var stream = inp.Open();
            var sr = new StreamReader(stream, Encoding.UTF8);

            while (!sr.EndOfStream)
            {
                var rawLine = await sr.ReadLineAsync();
                if (rawLine == null) break;
                var line = rawLine.Split('\x04');
                var names = GetArrayFromDelimitedText(':', line[structure.Author]);
                var authors = new List<Author>();
                foreach (var name in names)
                {
                    var author = GetArrayFromDelimitedText(',', name, false);
                    authors.Add(new Author
                    {
                        FirstName = SanitizeName(author.Length >= 2 ? author[1] : null),
                        MiddleName = SanitizeName(author.Length >= 3 ? author[2] : null),
                        LastName = SanitizeName(author[0]),
                    });
                }
                var meta = new List<MetaField>
                    {
                        new MetaField{ Name = "size", Value = line[structure.Size] },
                        new MetaField{ Name = "libid", Value = line[structure.LibId] },
                        new MetaField{ Name = "del", Value = (line[structure.Del] == "1").ToString() },
                    };
                if (structure.Keywords != -1)
                {
                    var keywords = GetArrayFromDelimitedText(':', line[structure.Keywords]);
                    foreach (var word in keywords)
                    {
                        meta.Add(new MetaField { Name = "keyword", Value = word });
                    }
                }
                var genresText = GetArrayFromDelimitedText(':', line[structure.Genre]);
                var genres = new List<Genre>();
                foreach (var s in genresText)
                {
                    var genreTuples = _genres.Localize(s);
                    foreach (var tuple in genreTuples)
                    {
                        genres.Add(new Genre { Name = tuple.Item1, Child = new Genre { Name = tuple.Item2 } });
                    }
                }

                var args = new Book
                {
                    Authors = authors,
                    Genres = genres,
                    Title = line[structure.Title],
                    Series = SanitizeName(line[structure.Series]),
                    SeriesNo = ParseSeriesNo(line[structure.SeriesNo]),
                    File = line[structure.File],
                    Ext = line[structure.Ext],
                    Date = DateTime.Parse(line[structure.Date]),
                    Language = structure.Language != -1 ? line[structure.Language] : null,
                    Archive = inp.Name.Replace(".inp", ".zip"),
                    Meta = meta
                };
                yield return args;
            }
        }
    }

    private async Task<InpStructure> GetStructure()
    {
        var structure = DefaultStructure;
        var info = _zip!.Entries.FirstOrDefault(entry => entry.Name.EndsWith("structure.info"));
        if (info != null)
        {
            using var reader = new StreamReader(info.Open(), _cp1251);
            structure = await reader.ReadToEndAsync();
        }

        var struc = GetArrayFromDelimitedText(';', structure);
        var result = new InpStructure
        {
            Author = Array.FindIndex(struc, r => r == "AUTHOR"),
            Genre = Array.FindIndex(struc, r => r == "GENRE"),
            Title = Array.FindIndex(struc, r => r == "TITLE"),
            Series = Array.FindIndex(struc, r => r == "SERIES"),
            SeriesNo = Array.FindIndex(struc, r => r == "SERNO"),
            File = Array.FindIndex(struc, r => r == "FILE"),
            Size = Array.FindIndex(struc, r => r == "SIZE"),
            LibId = Array.FindIndex(struc, r => r == "LIBID"),
            Del = Array.FindIndex(struc, r => r == "DEL"),
            Ext = Array.FindIndex(struc, r => r == "EXT"),
            Date = Array.FindIndex(struc, r => r == "DATE"),
            Language = Array.FindIndex(struc, r => r == "LANG"),
            Keywords = Array.FindIndex(struc, r => r == "KEYWORDS"),
        };

        return result;
    }

    private static string[] GetArrayFromDelimitedText(char delimiter, string text, bool removeEmpty = true) =>
        text
            .Split(delimiter)
            .Where(x => !removeEmpty || (!string.IsNullOrEmpty(x) || x == ","))
            .ToArray();

    private static string? SanitizeName(string? name)
    {
        if (name == null) return null;
        name = name.Trim();
        if (name == "" || name == "--")
            return null;
        return name;
    }

    private static int ParseSeriesNo(string s)
    {
        // flibusta contains a whole lot of broken ones like:
        // 1-4
        // 2.2
        // 1,2
        // IV
        // 2008 04
        // ?
        // « name=»Рассказы
        // Глава 5. За товаром
        if (int.TryParse(s, out int result))
        {
            return result;
        }
        return -1;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _zip!.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

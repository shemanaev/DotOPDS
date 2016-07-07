using DotOPDS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotOPDS.Utils
{
    public class NewEntryEventArgs : EventArgs
    {
        public Book Book;
    }
    public delegate void NewEntryEventHandler(object sender, NewEntryEventArgs e);
    public delegate void FinishedEventHandler(object sender);

    // http://forum.home-lib.net/index.php?showtopic=18
    // http://forum.home-lib.net/index.php?showtopic=16
    // http://forum.home-lib.net/index.php?showtopic=329&#entry2655
    // https://dotnetzip.codeplex.com/wikipage?title=CS-Examples&referringTitle=Examples
    public class InpxParser : IDisposable
    {
        private static readonly int[] fb2ids = { 0, 65536 };
        private const string defaultStructure = "AUTHOR;GENRE;TITLE;SERIES;SERNO;FILE;SIZE;LIBID;DEL;EXT;DATE;";
        private static Encoding CP1251 = Encoding.GetEncoding(1251);

        private readonly ZipArchive zip;
        private readonly string[] comment;

        public event NewEntryEventHandler OnNewEntry;
        public event FinishedEventHandler OnFinished;

        public bool IsFb2 => fb2ids.Contains(int.Parse(comment[2]));
        public string Version { get; private set; }
        public string Name => comment[0];
        public string FileName => comment[1];
        public string Description => string.Join("\n", comment, 3, comment.Length - 3);

        public InpxParser(string filename)
        {
            zip = ZipFile.Open(filename, ZipArchiveMode.Read, CP1251);
            using (var reader = new StreamReader(zip.GetEntry("collection.info").Open(), CP1251))
            {
                comment = reader.ReadToEnd().Split('\n');
            }
            var result = zip.Entries.First(entry => entry.Name.EndsWith("version.info"));
            if (result != null)
            {
                using (var reader = new StreamReader(result.Open(), CP1251))
                {
                    Version = reader.ReadToEnd().Trim();
                }
            }
        }

        public Task Parse()
        {
            return Task.Run((Action)ParseStart);
        }

        private void ParseStart()
        {
            var structure = GetStructure();

            var inps = zip.Entries.Where(entry => entry.Name.EndsWith(".inp"));
            foreach (var inp in inps)
            {
                using (var stream = inp.Open())
                {
                    var sr = new StreamReader(stream, Encoding.UTF8);

                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine().Split('\x04');
                        var names = GetDelimArray(':', line[structure.Author]);
                        var authors = new List<Author>();
                        foreach (var name in names)
                        {
                            var author = GetDelimArray(',', name, false);
                            authors.Add(new Author {
                                FirstName = SanitizeName(author.Length >= 2 ? author[1] : null),
                                MiddleName = SanitizeName(author.Length >= 3 ? author[2] : null),
                                LastName = SanitizeName(author[0]),
                            });
                        }
                        var args = new Book
                        {
                            Id = Guid.Empty,
                            Authors = authors.ToArray(),
                            Genres = GetDelimArray(':', line[structure.Genre]),
                            Title = line[structure.Title],
                            Series = SanitizeName(line[structure.Series]),
                            SeriesNo = ParseInt(line[structure.SeriesNo]),
                            File = line[structure.File],
                            Size = int.Parse(line[structure.Size]),
                            LibId = int.Parse(line[structure.LibId]),
                            Del = line[structure.Del] == "1",
                            Ext = line[structure.Ext],
                            Date = DateTime.Parse(line[structure.Date]),
                            Language = structure.Language != -1 ? line[structure.Language] : null,
                            Keywords = structure.Keywords != -1 ? GetDelimArray(':', line[structure.Keywords]) : null,
                            Archive = inp.Name.Replace(".inp", ".zip"),
                        };
                        OnNewEntry?.Invoke(this, new NewEntryEventArgs { Book = args });
                    }
                }
            }

            OnFinished?.Invoke(this);
        }

        private InpStructure GetStructure()
        {
            var structure = defaultStructure;
            var info = zip.Entries.FirstOrDefault(entry => entry.Name.EndsWith("structure.info"));
            if (info != null)
            {
                using (var reader = new StreamReader(info.Open(), CP1251))
                {
                    structure = reader.ReadToEnd();
                }
            }

            var struc = GetDelimArray(';', structure);
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

        private string[] GetDelimArray(char delimiter, string text, bool removeEmpty = true)
        {
            return text.Split(delimiter).Where(x => !removeEmpty || (!string.IsNullOrEmpty(x) || x == ",")).ToArray();
        }

        private string SanitizeName(string name)
        {
            if (name == null) return null;
            name = name.Trim();
            if (name == "" || name == "--")
                return null;
            return name;
        }

        private int ParseInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return -1;
            return int.Parse(s);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                zip.Dispose();
            }
        }
    }

    class InpStructure
    {
        public int Author { get; set; }
        public int Genre { get; set; }
        public int Title { get; set; }
        public int Series { get; set; }
        public int SeriesNo { get; set; }
        public int File { get; set; }
        public int Size { get; set; }
        public int LibId { get; set; }
        public int Del { get; set; }
        public int Ext { get; set; }
        public int Date { get; set; }
        public int Language { get; set; }
        public int Keywords { get; set; }
    }
}

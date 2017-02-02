using System;

namespace DotOPDS.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid LibraryId { get; set; }
        public Author[] Authors { get; set; }
        public string[] Genres { get; set; }
        public string Title { get; set; }
        public string Series { get; set; }
        public int SeriesNo { get; set; }
        public string File { get; set; }
        public int Size { get; set; }
        public int LibId { get; set; }
        public bool Del { get; set; }
        public string Ext { get; set; }
        public DateTime Date { get; set; }
        public string Language { get; set; }
        public string[] Keywords { get; set; }
        public string Archive { get; set; }
        public string Annotation { get; set; }
        public Cover Cover { get; set; }
        // for Pdf's
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Pages { get; set; }
    }
}

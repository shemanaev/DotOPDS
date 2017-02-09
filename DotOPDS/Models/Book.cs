using System;
using System.Collections.Generic;

namespace DotOPDS.Models
{
    public class Book
    {
        internal Guid LibraryId { get; set; }
        internal bool UpdatedFromFile { get; set; }
        internal DateTime UpdatedAt { get; set; }

        public Guid Id { get; internal set; }
        public string File { get; set; }                 // file path relative to library base without extension. required
        public string Ext { get; set; }                  // file extension. required
        public string Archive { get; set; }              // archive with file. optional
        public string Title { get; set; }                // book title. required
        public string Annotation { get; set; }           // book annotation. optional
        public DateTime Date { get; set; }               // publication date. required
        public string Series { get; set; }               // book series. optional
        public int SeriesNo { get; set; }                // number of book in series. optional
        public Cover Cover { get; set; }                 // cover image. optional
        public string Language { get; set; }             // language code (two letters). optional
        public IEnumerable<Author> Authors { get; set; } // list of authors. required
        public IEnumerable<Genre> Genres { get; set; }   // list of genres. optional
        public IEnumerable<MetaField> Meta { get; set; } // set of extra fields needs to be indexed. optional
    }
}

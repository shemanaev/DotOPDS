using System;
using System.Collections.Generic;

namespace DotOPDS.Models
{
    /// <summary>
    /// Represents book.
    /// </summary>
    public class Book
    {
        internal Guid LibraryId { get; set; }
        internal bool UpdatedFromFile { get; set; }
        internal DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Unique book id.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// File path relative to library base without extension. Required.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// File extension. Required.
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// Archive with file. Optional.
        /// </summary>
        public string Archive { get; set; }

        /// <summary>
        /// Book title. Required.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Book annotation. Optional.
        /// </summary>
        public string Annotation { get; set; }

        /// <summary>
        /// Publication date. Required.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Book series. Optional.
        /// </summary>
        public string Series { get; set; }

        /// <summary>
        /// Number of book in series. Optional.
        /// </summary>
        public int SeriesNo { get; set; }

        /// <summary>
        /// Cover image. Optional.
        /// </summary>
        public Cover Cover { get; set; }

        /// <summary>
        /// Language code (two letters). Optional.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// List of authors. Required.
        /// </summary>
        public IEnumerable<Author> Authors { get; set; }

        /// <summary>
        /// List of genres. Optional.
        /// </summary>
        public IEnumerable<Genre> Genres { get; set; }

        /// <summary>
        /// Set of extra fields needs to be indexed. Optional.
        /// </summary>
        public IEnumerable<MetaField> Meta { get; set; }
    }
}

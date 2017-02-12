namespace DotOPDS.Models
{
    /// <summary>
    /// Represents genre.
    /// Can be multi-level.
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nested genre or null if last.
        /// </summary>
        public Genre Child { get; set; }
    }
}

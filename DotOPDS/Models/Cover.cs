namespace DotOPDS.Models
{
    /// <summary>
    /// Represents book cover image.
    /// </summary>
    public class Cover
    {
        /// <summary>
        /// Actual image binary data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Image content type that stored in <see cref="Data"/>.
        /// </summary>
        public string ContentType { get; set; }
    }
}

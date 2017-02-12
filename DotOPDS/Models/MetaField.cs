namespace DotOPDS.Models
{
    /// <summary>
    /// Represents additional field that not fits into <see cref="Book"/> model
    /// but should be indexed.
    /// </summary>
    public class MetaField
    {
        /// <summary>
        /// Field name. Will be available for search.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Field value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Should <see cref="Value"/> be analyzed?
        /// I.e. tokenized and stemmed.
        /// </summary>
        public bool IsAnalyzed { get; set; }
    }
}

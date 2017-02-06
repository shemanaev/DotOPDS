namespace DotOPDS.Models
{
    public class MetaField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsAnalyzed { get; set; } // should it be tokenized or stored 'as is'?
    }
}

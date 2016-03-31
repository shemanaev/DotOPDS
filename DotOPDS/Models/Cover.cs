namespace DotOPDS.Models
{
    public class Cover
    {
        public bool? Has { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
    }
}

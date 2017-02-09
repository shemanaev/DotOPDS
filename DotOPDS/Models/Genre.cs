namespace DotOPDS.Models
{
    public class Genre
    {
        public string Name { get; set; }
        public Genre Child { get; set; } 
    }
}

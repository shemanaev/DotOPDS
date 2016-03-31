namespace DotOPDS.Models
{
    public class Author
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string GetScreenName()
        {
            string format = "";
            if (!string.IsNullOrEmpty(FirstName)) format += "{0}";
            if (!string.IsNullOrEmpty(MiddleName)) format += " {1}";
            if (!string.IsNullOrEmpty(LastName)) format += " {2}";
            return string.Format(format, FirstName, MiddleName, LastName).Trim();
        }
    }
}

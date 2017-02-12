namespace DotOPDS.Models
{
    internal static class AuthorExtensions
    {
        public static string GetScreenName(this Author author)
        {
            string format = "";
            if (!string.IsNullOrEmpty(author.FirstName)) format += "{0}";
            if (!string.IsNullOrEmpty(author.MiddleName)) format += " {1}";
            if (!string.IsNullOrEmpty(author.LastName)) format += " {2}";
            return string.Format(format, author.FirstName, author.MiddleName, author.LastName).Trim();
        }
    }
}

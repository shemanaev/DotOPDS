using DotOPDS.Contract.Models;
using System;
using System.Linq;

namespace DotOPDS.Extensions;

public static class GenreExtensions
{
    private const string SEPARATOR = "~~~";
    public static string GetFullName(this Genre genre) =>
        genre.Child != null ? $"{genre.Name!.Replace(SEPARATOR, "")}{SEPARATOR}{genre.Child.GetFullName()}" : genre.Name!.Replace(SEPARATOR, "");

    public static string GetDisplayName(this Genre genre) =>
        genre.Child != null ? genre.Child.GetDisplayName() : genre.Name!;

    public static string GetDisplayName(string genre) =>
        genre.Split(new[] { SEPARATOR }, StringSplitOptions.None).Last();

    public static string GetNthName(string genre, int index) =>
        genre.Split(new[] { SEPARATOR }, StringSplitOptions.None).ElementAtOrDefault(index)!;

    public static bool IsLast(string genre) =>
        !genre.Contains(SEPARATOR, StringComparison.CurrentCulture);

    public static string Combine(string s1, string s2) =>
        string.IsNullOrWhiteSpace(s1) ? s2 : $"{s1}{SEPARATOR}{s2}";

    public static string Cut(string genre, string basegenre) =>
        string.IsNullOrWhiteSpace(basegenre) ? genre : genre.Replace($"{basegenre}{SEPARATOR}", "");

    public static Genre Construct(string genre)
    {
        var s = genre.Split(new [] { SEPARATOR }, StringSplitOptions.None);
        var root = new Genre();
        var current = root;
        for (var i = 0; i < s.Length; i++)
        {
            current.Name = s[i];
            if (i < s.Length - 1)
            {
                current.Child = new Genre();
                current = current.Child;
            }
        }
        return root;
    }
}

using System.Xml.Serialization;

namespace DotOPDS.Tests;

public static class Extensions
{
    public static async Task<T> GetAndDeserialize<T>(this HttpClient client, string requestUri)
    {
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(result);
        return (T)serializer.Deserialize(reader)!;
    }
}

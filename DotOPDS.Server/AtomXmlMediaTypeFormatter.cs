using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;

namespace DotOPDS;

/// <summary>
/// This class handles serialization of objects
/// to XML using <see cref="XmlSerializer"/>
/// </summary>
public class AtomXmlMediaTypeFormatter : XmlSerializerOutputFormatter
{
    private readonly MediaTypeHeaderValue _mediaType = new("application/atom+xml");
    private readonly XmlSerializerNamespaces _serializerNamespaces;

    public AtomXmlMediaTypeFormatter()
    {
        SupportedMediaTypes.Insert(0, _mediaType);

        _serializerNamespaces = new XmlSerializerNamespaces();
        _serializerNamespaces.Add("dc", "http://purl.org/dc/terms/");
        _serializerNamespaces.Add("os", "http://a9.com/-/spec/opensearch/1.1/");
        _serializerNamespaces.Add("opds", "http://opds-spec.org/2010/catalog");
        _serializerNamespaces.Add("dotopds", "urn:dotopds:v1.0");
    }

    /// <summary>
    /// Serializes value using the passed in <paramref name="xmlSerializer"/> and <paramref name="xmlWriter"/>.
    /// </summary>
    /// <param name="xmlSerializer">The serializer used to serialize the <paramref name="value"/>.</param>
    /// <param name="xmlWriter">The writer used by the serializer <paramref name="xmlSerializer"/>
    /// to serialize the <paramref name="value"/>.</param>
    /// <param name="value">The value to be serialized.</param>
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object? value)
    {
        xmlSerializer.Serialize(xmlWriter, value, _serializerNamespaces);
    }
}

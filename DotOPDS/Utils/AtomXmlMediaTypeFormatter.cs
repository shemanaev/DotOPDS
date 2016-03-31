using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DotOPDS.Utils
{
    public class AtomXmlMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly MediaTypeHeaderValue _mediaType = new MediaTypeHeaderValue("application/atom+xml");
        private readonly XmlWriterSettings _xmlWriterSettings;
        private readonly XmlSerializerNamespaces _serializerNamespaces;

        public Dictionary<Type, XmlSerializer> Serializers { get; set; }

        public AtomXmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(_mediaType);
            SupportedEncodings.Add(Encoding.UTF8);

            Serializers = new Dictionary<Type, XmlSerializer>();

            _xmlWriterSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false), // no BOM, please
#if DEBUG
                Indent = true,
#endif
            };

            _serializerNamespaces = new XmlSerializerNamespaces();
            _serializerNamespaces.Add("dc", "http://purl.org/dc/terms/");
            _serializerNamespaces.Add("os", "http://a9.com/-/spec/opensearch/1.1/");
            _serializerNamespaces.Add("opds", "http://opds-spec.org/2010/catalog");
            _serializerNamespaces.Add("dotopds", "urn:dotopds:v1.0");
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            lock (Serializers)
            {
                if (!Serializers.ContainsKey(type))
                {
                    var serializer = new XmlSerializer(type);
                    Serializers.Add(type, serializer);
                }
            }

            return Task.Factory.StartNew(() =>
            {
                XmlSerializer serializer;
                lock (Serializers)
                {
                    serializer = Serializers[type];
                }

                using (var writer = XmlWriter.Create(writeStream, _xmlWriterSettings))
                {
                    serializer.Serialize(writer, value, _serializerNamespaces);
                }
            });
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }
    }
}

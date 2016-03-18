using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DotOPDS.Utils
{
    public class AtomXmlMediaTypeFormatter : XmlMediaTypeFormatter
    {
        private readonly MediaTypeHeaderValue _mediaType = new MediaTypeHeaderValue("application/xml");
        private readonly XmlWriterSettings _xmlWriterSettings;
        private readonly XmlSerializerNamespaces _serializerNamespaces;

        public Dictionary<Type, XmlSerializer> Serializers { get; set; }

        public AtomXmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(_mediaType);
            SupportedEncodings.Add(Encoding.UTF8);

            Serializers = new Dictionary<Type, XmlSerializer>();

            _xmlWriterSettings = new XmlWriterSettings { OmitXmlDeclaration = true };

#if DEBUG
            _xmlWriterSettings.Indent = true;
#endif

            _serializerNamespaces = new XmlSerializerNamespaces();
            _serializerNamespaces.Add("dc", "http://purl.org/dc/terms/");
            _serializerNamespaces.Add("os", "http://a9.com/-/spec/opensearch/1.1/");
            _serializerNamespaces.Add("opds", "http://opds-spec.org/2010/catalog");
        }

        public override Task WriteToStreamAsync(Type type, object value, System.IO.Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
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
    }
}

using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace DotOPDS.Utils
{
    public static class EmbedioExtensions
    {
        /// <summary>
        /// Outputs a Atom XML Response given a data object.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static bool FeedResponse(this HttpListenerContext context, object data)
        {
            context.Response.ContentType = "application/atom+xml";

            var ns = new XmlSerializerNamespaces();
            ns.Add("dc", "http://purl.org/dc/terms/");
            ns.Add("os", "http://a9.com/-/spec/opensearch/1.1/");
            ns.Add("opds", "http://opds-spec.org/2010/catalog");

            var settings = new XmlWriterSettings();
#if DEBUG
            settings.Indent = true;
#endif
            var serializer = new XmlSerializer(data.GetType());
            using (var writer = XmlWriter.Create(context.Response.OutputStream, settings))
            {
                serializer.Serialize(writer, data, ns);
            }

            return true;
        }

        /// <summary>
        /// Gets the value for the specified query string key.
        /// Native HttpListenerRequest's implementation seems to be broken and
        /// decodes with ContentEncoding instead of UTF-8.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Query(this HttpListenerContext context, string key)
        {
            var queryString = HttpUtility.ParseQueryString(context.Request.Url.Query);
            return queryString.AllKeys.Contains(key) ? queryString[key] : null;
        }

        /// <summary>
        /// Gets the URL decoded path segments.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string[] Segments(this HttpListenerContext context)
        {
            return context.Request.Url.Segments.Select(x => HttpUtility.UrlDecode(x)).ToArray();
        }
    }
}

using System.Linq;
using System.Xml.Linq;

namespace DotOPDS.Plugin.FileFormat.Fb2
{
    static class XmlExtensions
    {
        public static XAttribute IngoreAttributeNamespace(this XAttribute attrib)
        {
            var name = attrib.Name.LocalName;
            return new XAttribute(name, attrib.Value);
        }

        public static XElement IgnoreNamespace(this XElement xelem)
        {
            var name = xelem.Name.LocalName;
            var result = new XElement(name,
                                from e in xelem.Elements()
                                select e.IgnoreNamespace(),
                                from e in xelem.Attributes()
                                select e.IngoreAttributeNamespace()
            );
            if (!xelem.HasElements)
                result.Value = xelem.Value;
            return result;
        }
    }
}

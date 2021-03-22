using System.Xml.Linq;

namespace Xpand.Utils.Helpers {
    public static class StringExtensions {
        public static string GetAttributeValue(this XElement element, XName name) {
            XAttribute xAttribute = element.Attribute(name);
            return xAttribute?.Value;
        }
    }
}
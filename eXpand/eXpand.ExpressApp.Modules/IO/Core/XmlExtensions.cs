using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.Core {
    public static class XmlExtensions
    {
        public static IEnumerable<XElement> KeyElements(this IEnumerable<XElement> element, string name) {
            return element.Descendants("Key").Where(xElement => xElement.GetAttributeValue("name") == name);
        }
//        public static XElement ObjectNaturalKeyProperty(this XElement element)
//        {
//            return element.Descendants("Property").Where(xElement => xElement.GetAttributeValue("isNaturalKey") == "true").FirstOrDefault();
//        }

        public static IEnumerable<XElement> ObjectKeyProperties(this XElement element)
        {
            return element.Descendants("Property").Where(xElement => xElement.GetAttributeValue("isKey") == "true");
        }

        public static IEnumerable<XElement> Properties(this IEnumerable<XElement> elements, NodeType nodeType)
        {
            return elements.Descendants("Property").Where(xElement => xElement.GetAttributeValue("type") == nodeType.ToString().MakeFirstCharLower());
        }

        public static IEnumerable<XElement> Properties(this XElement element) {
            return element.Descendants("Property");
        }

        public static IEnumerable<XElement> Properties(this XElement element, NodeType nodeType) {
            return element.Properties().Where(xElement => xElement.GetAttributeValue("type") == nodeType.ToString().MakeFirstCharLower());
        }


        public static XElement Property(this XElement element, string name) {
            return element.Properties().Where(xElement => xElement.GetAttributeValue("name") == name).FirstOrDefault();
        }

        public static XElement Property(this IEnumerable<XElement> properties, string name) {
            return properties.Where(element => element.GetAttributeValue("name") == name).FirstOrDefault();
        }

        public static XElement ObjectProperty(this XElement element, Type type) {
            return element.Descendants("Property").Where(xElement =>
                                                          xElement.GetAttributeValue("type") ==
                                                          NodeType.Object.ToString().MakeFirstCharLower()).FirstOrDefault();
        }
        public static XElement ObjectProperty(this XElement element, string name) {
            return element.Descendants("Property").Where(xElement =>xElement.GetAttributeValue("name") ==name).FirstOrDefault();
        }

        public static XElement ObjectProperty(this IEnumerable<XElement> elements, Type type) {
            return elements.Select(element => element.ObjectProperty(type)).FirstOrDefault();
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element, Type type)
        {
            return SerializedObjects(element, type.Name);
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this IEnumerable<XElement> elements, SerializationStrategy serializationStrategy) {
            return elements.Where(element => element.GetAttributeValue("strategy")==serializationStrategy.ToString());
        }


        public static IEnumerable<XElement> SerializedObjectRefs(this XElement element, Type type)
        {
            return SerializedObjectRefs(element).Where(xElement => xElement.GetAttributeValue("type")==type.Name);
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this IEnumerable<XElement> elements) {
            return elements.Descendants("SerializedObjectRef");
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this XElement element) {
            return element.Descendants("SerializedObjectRef");
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element) {
            return element.Descendants("SerializedObject");
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element, string typeName)
        {
            return element.SerializedObjects().Where(xElement => xElement.GetAttributeValue("type") == typeName);
        }

        public static XElement FindObjectFromRefenceElement(this XElement xElement) {
            var typeValue = xElement.GetAttributeValue("type");
            var infos = xElement.Descendants("Key").Select(
                element1 => new { Element = element1, Name = element1.GetAttributeValue("name"), element1.Value });
            if (xElement.Document != null && xElement.Document.Root != null)
            {
                var @select =
                    xElement.Document.Root.SerializedObjects().Where(
                        element => element.GetAttributeValue("type") == typeValue).Descendants("Property").Where(
                        xElement1 => xElement1.GetAttributeValue("isKey") == "true").
                        Select(element1 =>
                        new { Element = element1.Parent, Name = element1.GetAttributeValue("name"), element1.Value });
                return
                    @select.Where(
                        arg => infos.Where(arg1 => arg.Name == arg1.Name && arg.Value == arg1.Value).Count() > 0).
                        Select(arg2 => arg2.Element).FirstOrDefault();
            }
            return null;

        }
    }
}
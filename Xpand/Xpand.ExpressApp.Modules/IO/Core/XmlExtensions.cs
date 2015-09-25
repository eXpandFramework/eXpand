using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Core {
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

        public static XElement ObjectProperty(this XElement element, string name) {
            return element.Descendants("Property").Where(xElement =>xElement.GetAttributeValue("name") ==name).FirstOrDefault();
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
        
    }
}
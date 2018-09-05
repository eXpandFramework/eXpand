using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Core {
    public static class XmlExtensions{
        public static IEnumerable<XElement> KeyElements(this IEnumerable<XElement> element, string name,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.Key).Where(xElement => xElement.GetAttributeValue(elementSchema.Name) == name);
        }

        public static IEnumerable<XElement> ObjectKeyProperties(this XElement element,bool minifyOutput){
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.Property).Where(xElement => xElement.GetAttributeValue(elementSchema.IsKey) == "true");
        }

        public static IEnumerable<XElement> Properties(this IEnumerable<XElement> elements, NodeType nodeType,bool minifyOutput){
            var elementSchema = ElementSchema.Get(minifyOutput);
            return elements.Descendants(elementSchema.Property).Where(xElement => xElement.GetTypeAttributeValue(minifyOutput) == nodeType.ToString().MakeFirstCharLower());
        }

        public static IEnumerable<XElement> Properties(this XElement element,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.Property);
        }

        public static string GetTypeAttributeValue(this XElement element, bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            var value = element.GetAttributeValue(elementSchema.Type);
            return string.IsNullOrEmpty(value) ? NodeType.Simple.ToString().MakeFirstCharLower() : value;
        }

        public static IEnumerable<XElement> Properties(this XElement element, NodeType nodeType,bool minifyOutput) {
            return element.Properties(minifyOutput).Where(xElement => xElement.GetTypeAttributeValue(minifyOutput) == nodeType.ToString().MakeFirstCharLower());
        }

        public static XElement Property(this XElement element, string name,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Properties(minifyOutput).FirstOrDefault(xElement => xElement.GetAttributeValue(elementSchema.Name) == name);
        }

        public static XElement Property(this IEnumerable<XElement> properties, string name,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return properties.FirstOrDefault(element => element.GetAttributeValue(elementSchema.Name) == name);
        }

        public static XElement ObjectProperty(this XElement element, string name,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.Property).FirstOrDefault(xElement => xElement.GetAttributeValue(elementSchema.Name) ==name);
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element, Type type,bool minifyOutput){
            return SerializedObjects(element, type.Name,minifyOutput);
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this IEnumerable<XElement> elements, SerializationStrategy serializationStrategy,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return elements.Where(element => element.GetAttributeValue(elementSchema.Strategy)==serializationStrategy.ToString());
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this XElement element, Type type,bool minifyOutput){
            return SerializedObjectRefs(element,minifyOutput).Where(xElement => xElement.GetTypeAttributeValue(minifyOutput)==type.Name);
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this IEnumerable<XElement> elements,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return elements.Descendants(elementSchema.SerializedObjectRef);
        }

        public static IEnumerable<XElement> SerializedObjectRefs(this XElement element,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.SerializedObjectRef);
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            return element.Descendants(elementSchema.SerializedObject);
        }

        public static IEnumerable<XElement> SerializedObjects(this XElement element, string typeName,bool minifyOutput){
            return element.SerializedObjects(minifyOutput).Where(xElement => xElement.GetTypeAttributeValue(minifyOutput) == typeName);
        }

        public static XElement FindObjectFromRefenceElement(this XElement xElement,bool minifyOutput) {
            var elementSchema = ElementSchema.Get(minifyOutput);
            var typeValue = xElement.GetTypeAttributeValue(minifyOutput);
            var infos = xElement.Descendants(elementSchema.Key).Select(
                element1 => new { Element = element1, Name = element1.GetAttributeValue(elementSchema.Name), element1.Value });
            var select =
                xElement.Document?.Root?.SerializedObjects(minifyOutput).Where(
                        element => element.GetTypeAttributeValue(minifyOutput) == typeValue).Descendants(elementSchema.Property).Where(
                        xElement1 => xElement1.GetAttributeValue(elementSchema.IsKey) == "true").
                    Select(element1 =>
                        new { Element = element1.Parent, Name = element1.GetAttributeValue(elementSchema.Name), element1.Value });
            return
                select?.Where(
                        arg => infos.Any(arg1 => arg.Name == arg1.Name && arg.Value == arg1.Value)).
                    Select(arg2 => arg2.Element).FirstOrDefault();
        }
    }
}
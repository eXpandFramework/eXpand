using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Linq;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Core {
    public class ExportEngine {
        readonly Dictionary<ObjectInfo, object> exportedObjecs = new Dictionary<ObjectInfo, object>();
        SerializeClassInfoGraphNodesCalculator _serializeClassInfoGraphNodesCalculator;

        public XDocument Export(IEnumerable<XPBaseObject> baseCollection,
                                ISerializationConfigurationGroup serializationConfigurationGroup) {
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            xDocument.Add(root);
            _serializeClassInfoGraphNodesCalculator =
                new SerializeClassInfoGraphNodesCalculator(serializationConfigurationGroup);
            foreach (var baseObject in baseCollection) {
                IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes =
                    _serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(baseObject);
                ExportCore(baseObject, serializedClassInfoGraphNodes, root);
            }
            return xDocument;

        }


        void ExportCore(XPBaseObject selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes,
                        XElement root) {
            var objectInfo = new ObjectInfo(selectedObject.GetType(),
                                            selectedObject.ClassInfo.KeyProperty.GetValue(selectedObject));
            if (!(exportedObjecs.ContainsKey(objectInfo))) {
                exportedObjecs.Add(objectInfo, null);
                var serializedObjectElement = new XElement("SerializedObject");
                serializedObjectElement.Add(new XAttribute("type", selectedObject.GetType().Name));
                root.Add(serializedObjectElement);
                foreach (
                    var classInfoGraphNode in
                        serializedClassInfoGraphNodes.Where(
                            node => node.SerializationStrategy != SerializationStrategy.DoNotSerialize)) {
                    XElement propertyElement = GetPropertyElement(serializedObjectElement, classInfoGraphNode);
                    switch (classInfoGraphNode.NodeType) {
                        case NodeType.Simple:
                            SetMemberValue(selectedObject, classInfoGraphNode, propertyElement);
                            break;
                        case NodeType.Object:
                            CreateObjectProperty(selectedObject, propertyElement, classInfoGraphNode, root);
                            break;
                        case NodeType.Collection:
                            CreateCollectionProperty(selectedObject, classInfoGraphNode, root, propertyElement);
                            break;
                    }
                }
            }
        }

        void SetMemberValue(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode,
                            XElement propertyElement) {
            var xpMemberInfo = selectedObject.ClassInfo.FindMember(classInfoGraphNode.Name);
            if (xpMemberInfo != null) {
                var memberValue = selectedObject.GetMemberValue(classInfoGraphNode.Name);
                if (xpMemberInfo.Converter != null)
                    memberValue = (xpMemberInfo.Converter.ConvertToStorageType(memberValue));
                if (memberValue is byte[])
                    memberValue = Convert.ToBase64String((byte[])memberValue);
                if (memberValue is DateTime)
                    memberValue = ((DateTime)memberValue).Ticks;

                if (memberValue is string) {
                    memberValue = IAFModule.SanitizeXmlString((string)memberValue);
                    propertyElement.Add(new XCData(memberValue.ToString()));
                } else {
                    propertyElement.Value = GetInvariantValue(memberValue);
                }
            }
        }

        string GetInvariantValue(object memberValue) {
            double result;
            var parse = Double.TryParse(memberValue + "", out result);
            return parse ? ((IConvertible)memberValue).ToString(CultureInfo.InvariantCulture) : memberValue + "";
        }

        XElement GetPropertyElement(XElement serializedObjectElement, IClassInfoGraphNode classInfoGraphNode) {
            var propertyElement = new XElement("Property");
            serializedObjectElement.Add(propertyElement);
            propertyElement.Add(new XAttribute("type", classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute("name", classInfoGraphNode.Name));
            propertyElement.Add(new XAttribute("isKey", classInfoGraphNode.Key));
            return propertyElement;
        }

        void CreateCollectionProperty(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root,
                                      XElement propertyElement) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo != null) {
                var theObjects = memberInfo.GetValue(selectedObject) as IEnumerable;
                if (theObjects != null)
                    foreach (XPBaseObject theObject in theObjects) {
                        CreateRefElelement(classInfoGraphNode, theObject.GetType().Name, root, theObject,
                                           propertyElement);
                    }
            }
        }

        void CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, string typeName, XElement root,
                                XPBaseObject theObject, XElement propertyElement) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            propertyElement.Add(serializedObjectRefElement);
            serializedObjectRefElement.Add(new XAttribute("type", typeName));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            if (theObject != null) {
                IEnumerable<IClassInfoGraphNode> classInfoGraphNodes =
                    _serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(theObject, typeName);
                CreateRefKeyElements(classInfoGraphNodes, theObject, serializedObjectRefElement);
                if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                    ExportCore(theObject, classInfoGraphNodes, root);
            }
        }

        void CreateRefKeyElements(IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XPBaseObject theObject,
                                  XElement serializedObjectRefElement) {
            foreach (var infoGraphNode in serializedClassInfoGraphNodes.Where(node => node.Key)) {
                var serializedObjectRefKeyElement = new XElement("Key");
                serializedObjectRefKeyElement.Add(new XAttribute("name", infoGraphNode.Name));
                serializedObjectRefKeyElement.Value = theObject.GetMemberValue(infoGraphNode.Name).ToString();
                serializedObjectRefElement.Add(serializedObjectRefKeyElement);
            }
        }

        void CreateObjectProperty(XPBaseObject selectedObject, XElement propertyElement,
                                  IClassInfoGraphNode classInfoGraphNode, XElement root) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo != null) {
                var theObject = (XPBaseObject)memberInfo.GetValue(selectedObject);
                CreateRefElelement(classInfoGraphNode,
                                   theObject != null ? theObject.GetType().Name : memberInfo.MemberType.Name, root,
                                   theObject, propertyElement);
            }
        }


        public void Export(IEnumerable<XPBaseObject> xpBaseObjects,
                           ISerializationConfigurationGroup serializationConfigurationGroup, string fileName) {
            var document = Export(xpBaseObjects, serializationConfigurationGroup);
            if (fileName != null) {
                var xmlWriterSettings = new XmlWriterSettings {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineChars = "\r\n",
                    CloseOutput = true,
                };
                using (
                    XmlWriter textWriter = XmlWriter.Create(new FileStream(fileName, FileMode.Create), xmlWriterSettings)
                    ) {
                    document.Save(textWriter);
                    textWriter.Close();
                }
            }
        }
    }

    public class IAFModule {
        public static string SanitizeXmlString(string xml) {
            if (xml == null) {
                throw new ArgumentNullException("xml");
            }
            var buffer = new StringBuilder(xml.Length);
            foreach (char c in xml.Where(c => IsLegalXmlChar(c))) {
                buffer.Append(c);
            }
            return buffer.ToString().Trim();
        }
        public static bool IsLegalXmlChar(int character) {
            return (
            character == 0x9 /* == '\t' == 9 */ ||
            character == 0xA /* == '\n' == 10 */ ||
            character == 0xD /* == '\r' == 13 */ ||
            (character >= 0x20 && character <= 0xD7FF) ||
            (character >= 0xE000 && character <= 0xFFFD) ||
            (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }

}
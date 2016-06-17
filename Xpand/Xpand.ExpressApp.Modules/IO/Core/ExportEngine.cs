using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Core {
    public class ExportEngine {
        private readonly IObjectSpace _objectSpace;
        private readonly Dictionary<ObjectInfo, object> _exportedObjecs = new Dictionary<ObjectInfo, object>();
        SerializeClassInfoGraphNodesCalculator _serializeClassInfoGraphNodesCalculator;

        public ExportEngine(IObjectSpace objectSpace){
            _objectSpace = objectSpace;
        }

        public XDocument Export(IEnumerable<object> baseCollection,
                                ISerializationConfigurationGroup serializationConfigurationGroup) {
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            xDocument.Add(root);
            _serializeClassInfoGraphNodesCalculator =
                new SerializeClassInfoGraphNodesCalculator(serializationConfigurationGroup,_objectSpace);
            foreach (var baseObject in baseCollection) {
                var serializedClassInfoGraphNodes =_serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(baseObject);
                ExportCore(baseObject, serializedClassInfoGraphNodes, root);
            }
            return xDocument;

        }


        void ExportCore(object selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes,
                        XElement root) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(selectedObject.GetType());
            var objectInfo = new ObjectInfo(selectedObject.GetType(),typeInfo.KeyMember.GetValue(selectedObject));
            if (!(_exportedObjecs.ContainsKey(objectInfo))) {
                _exportedObjecs.Add(objectInfo, null);
                var serializedObjectElement = new XElement("SerializedObject");
                serializedObjectElement.Add(new XAttribute("type", selectedObject.GetType().Name));
                root.Add(serializedObjectElement);
                foreach (var classInfoGraphNode in serializedClassInfoGraphNodes.Where(
                            node => node.SerializationStrategy != SerializationStrategy.DoNotSerialize)) {
                    XElement propertyElement = GetPropertyElement(serializedObjectElement, classInfoGraphNode);
                    switch (classInfoGraphNode.NodeType) {
                        case NodeType.Simple:
                            SetMemberValue(typeInfo, selectedObject, classInfoGraphNode, propertyElement);
                            break;
                        case NodeType.Object:
                            CreateObjectProperty(typeInfo,selectedObject, propertyElement, classInfoGraphNode, root);
                            break;
                        case NodeType.Collection:
                            CreateCollectionProperty(typeInfo,selectedObject, classInfoGraphNode, root, propertyElement);
                            break;
                    }
                }
            }
        }

        void SetMemberValue(ITypeInfo typeInfo, object theObject, IClassInfoGraphNode classInfoGraphNode, XElement propertyElement) {
            var memberInfo = typeInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo != null) {
                var memberValue = memberInfo.GetValue(theObject);
                var valueConverter = memberInfo.FindAttribute<ValueConverterAttribute>();
                if (valueConverter != null)
                    memberValue = (valueConverter.Converter.ConvertToStorageType(memberValue));
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

        void CreateCollectionProperty(ITypeInfo typeInfo, object selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root, XElement propertyElement) {
            var memberInfo = typeInfo.FindMember(classInfoGraphNode.Name);
            var theObjects = memberInfo?.GetValue(selectedObject) as IEnumerable;
            if (theObjects != null)
                foreach (var theObject in theObjects) {
                    CreateRefElelement(classInfoGraphNode, theObject.GetType(), root, theObject,propertyElement);
                }
        }

        void CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, Type objectType, XElement root, object theObject, XElement propertyElement) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            propertyElement.Add(serializedObjectRefElement);
            serializedObjectRefElement.Add(new XAttribute("type", objectType.Name));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            if (theObject != null) {
                var classInfoGraphNodes =_serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(theObject, objectType.Name).ToArray();
                CreateRefKeyElements(XafTypesInfo.CastTypeToTypeInfo(objectType), classInfoGraphNodes, theObject, serializedObjectRefElement);
                if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                    ExportCore(theObject, classInfoGraphNodes, root);
            }
        }

        void CreateRefKeyElements(ITypeInfo typeInfo, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, object theObject, XElement serializedObjectRefElement) {
            foreach (var infoGraphNode in serializedClassInfoGraphNodes.Where(node => node.Key)) {
                var serializedObjectRefKeyElement = new XElement("Key");
                serializedObjectRefKeyElement.Add(new XAttribute("name", infoGraphNode.Name));
                serializedObjectRefKeyElement.Value = typeInfo.FindMember(infoGraphNode.Name).GetValue(theObject)+"";
                serializedObjectRefElement.Add(serializedObjectRefKeyElement);
            }
        }

        void CreateObjectProperty(ITypeInfo typeInfo, object selectedObject, XElement propertyElement, IClassInfoGraphNode classInfoGraphNode, XElement root) {
            var memberInfo = typeInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo != null) {
                var theObject = (XPBaseObject)memberInfo.GetValue(selectedObject);
                CreateRefElelement(classInfoGraphNode, theObject?.GetType() ?? memberInfo.MemberType, root,theObject, propertyElement);
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
                throw new ArgumentNullException(nameof(xml));
            }
            var buffer = new StringBuilder(xml.Length);
            foreach (char c in xml.Where(c => IsLegalXmlChar(c))) {
                buffer.Append(c);
            }
            return buffer.ToString();
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
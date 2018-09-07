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
using Fasterflect;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.IO.Core {
    public static class ExportEngineExtensions {
        internal static bool HasDefaultValue<T>(this T memberInfo, object memberValue) where T:IMemberInfo {
            if (memberInfo.MemberTypeInfo.IsPersistent)
                return memberValue == null;
            if (memberInfo.MemberTypeInfo.Type == typeof(string))
                return memberValue==null;
            if (memberInfo.MemberTypeInfo.Type.IsValueType)
                return memberInfo.MemberTypeInfo.Type.CreateInstance() == memberValue;
            return memberValue == null;
        }

        public static bool IsMinified(this XDocument document) {
            var value = document.Root.GetAttributeValue(ElementSchema.Minified);
            return value=="true";
        }


    }
    public class ExportEngine {
        public static XmlWriterSettings GetXMLWriterSettings(bool minifyOutput){
            var xmlWriterSettings = new XmlWriterSettings{
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineChars = "\r\n",
                CloseOutput = false,
            };
            if (minifyOutput) {
                xmlWriterSettings.Indent = false;
                xmlWriterSettings.NamespaceHandling=NamespaceHandling.Default;
                xmlWriterSettings.OmitXmlDeclaration = false;
                xmlWriterSettings.NewLineChars = "";
            }
            return xmlWriterSettings;
        }

        private readonly IObjectSpace _objectSpace;
        private readonly Dictionary<ObjectInfo, object> _exportedObjecs = new Dictionary<ObjectInfo, object>();
        SerializeClassInfoGraphNodesCalculator _serializeClassInfoGraphNodesCalculator;
        private bool _minifyOutput;

        public ExportEngine(IObjectSpace objectSpace){
            _objectSpace = objectSpace;
        }

        public XDocument Export(IEnumerable<object> baseCollection,
                                ISerializationConfigurationGroup serializationConfigurationGroup) {
            var xDocument = new XDocument();
            _minifyOutput = serializationConfigurationGroup.MinifyOutput;
            var elementInfo = ElementSchema.Get(_minifyOutput);
            var root = new XElement(elementInfo.SerializedObjects);
            root.Add(new XAttribute(ElementSchema.Minified,_minifyOutput));
            xDocument.Add(root);
            _serializeClassInfoGraphNodesCalculator =
                new SerializeClassInfoGraphNodesCalculator(serializationConfigurationGroup,_objectSpace);
            foreach (var baseObject in baseCollection) {
                var serializedClassInfoGraphNodes =_serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(baseObject);
                ExportCore(baseObject, serializedClassInfoGraphNodes, root,elementInfo);
            }
            return xDocument;

        }


        void ExportCore(object selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes,
            XElement root, ElementSchema elementSchema) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(selectedObject.GetType());
            var objectInfo = new ObjectInfo(selectedObject.GetType(),typeInfo.KeyMember.GetValue(selectedObject));
            if (!(_exportedObjecs.ContainsKey(objectInfo))) {
                _exportedObjecs.Add(objectInfo, null);
                var serializedObjectElement = new XElement(elementSchema.SerializedObject);
                serializedObjectElement.Add(new XAttribute(elementSchema.Type, selectedObject.GetType().Name));
                root.Add(serializedObjectElement);
                foreach (var classInfoGraphNode in serializedClassInfoGraphNodes.Where(
                            node => node.SerializationStrategy != SerializationStrategy.DoNotSerialize)) {
                    var propertyElement = GetPropertyElement(serializedObjectElement, classInfoGraphNode,elementSchema);
                    switch (classInfoGraphNode.NodeType) {
                        case NodeType.Simple:
                            SetMemberValue(typeInfo, selectedObject, classInfoGraphNode, propertyElement);
                            break;
                        case NodeType.Object:
                            CreateObjectProperty(typeInfo,selectedObject, propertyElement, classInfoGraphNode, root,elementSchema);
                            break;
                        case NodeType.Collection:
                            CreateCollectionProperty(typeInfo,selectedObject, classInfoGraphNode, root, propertyElement,elementSchema);
                            break;
                    }
                }
            }
        }

        void SetMemberValue(ITypeInfo typeInfo, object theObject, IClassInfoGraphNode classInfoGraphNode,
            XElement propertyElement) {
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
                if ((_minifyOutput&&!memberInfo.HasDefaultValue(memberValue))||!_minifyOutput ) {
                    if (memberValue is string) {
                        memberValue = IAFModule.SanitizeXmlString((string)memberValue);
                        propertyElement.Add(new XCData(memberValue.ToString()));
                    } else {
                        propertyElement.Value = GetInvariantValue(memberValue);
                    }
                }
                else {
                    propertyElement.Remove();
                }
            }
        }

        string GetInvariantValue(object memberValue) {
            var parse = Double.TryParse(memberValue + "", out _);
            return parse ? ((IConvertible)memberValue).ToString(CultureInfo.InvariantCulture) : memberValue + "";
        }

        XElement GetPropertyElement(XElement serializedObjectElement, IClassInfoGraphNode classInfoGraphNode,
            ElementSchema elementSchema) {
            var propertyElement = new XElement(elementSchema.Property);
            serializedObjectElement.Add(propertyElement);
            if ((_minifyOutput&&classInfoGraphNode.NodeType != NodeType.Simple)||!_minifyOutput)
                propertyElement.Add(new XAttribute(elementSchema.Type, classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute(elementSchema.Name, classInfoGraphNode.Name));
            if ((_minifyOutput&&classInfoGraphNode.Key)||!_minifyOutput)
                propertyElement.Add(new XAttribute(elementSchema.IsKey, classInfoGraphNode.Key));
            return propertyElement;
        }

        void CreateCollectionProperty(ITypeInfo typeInfo, object selectedObject, IClassInfoGraphNode classInfoGraphNode,
            XElement root, XElement propertyElement, ElementSchema elementSchema) {
            var memberInfo = typeInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo?.GetValue(selectedObject) is IEnumerable theObjects)
                foreach (var theObject in theObjects) {
                    CreateRefElelement(classInfoGraphNode, theObject.GetType(), root, theObject,propertyElement,elementSchema);
                }
        }

        void CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, Type objectType, XElement root,
            object theObject, XElement propertyElement, ElementSchema elementSchema) {
            var serializedObjectRefElement = new XElement(elementSchema.SerializedObjectRef);
            propertyElement.Add(serializedObjectRefElement);
            serializedObjectRefElement.Add(new XAttribute(elementSchema.Type, objectType.Name));
            serializedObjectRefElement.Add(new XAttribute(elementSchema.Strategy, classInfoGraphNode.SerializationStrategy));
            if (theObject != null) {
                var classInfoGraphNodes =_serializeClassInfoGraphNodesCalculator.GetSerializedClassInfoGraphNodes(theObject, objectType.Name).ToArray();
                CreateRefKeyElements(XafTypesInfo.CastTypeToTypeInfo(objectType), classInfoGraphNodes, theObject, serializedObjectRefElement,elementSchema);
                if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                    ExportCore(theObject, classInfoGraphNodes, root,elementSchema);
            }
        }

        void CreateRefKeyElements(ITypeInfo typeInfo, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes,
            object theObject, XElement serializedObjectRefElement, ElementSchema elementSchema) {
            foreach (var infoGraphNode in serializedClassInfoGraphNodes.Where(node => node.Key)) {
                var serializedObjectRefKeyElement = new XElement(elementSchema.Key);
                serializedObjectRefKeyElement.Add(new XAttribute(elementSchema.Name, infoGraphNode.Name));
                serializedObjectRefKeyElement.Value = typeInfo.FindMember(infoGraphNode.Name).GetValue(theObject)+"";
                serializedObjectRefElement.Add(serializedObjectRefKeyElement);
            }
        }

        void CreateObjectProperty(ITypeInfo typeInfo, object selectedObject, XElement propertyElement,
            IClassInfoGraphNode classInfoGraphNode, XElement root, ElementSchema elementSchema) {
            var memberInfo = typeInfo.FindMember(classInfoGraphNode.Name);
            if (memberInfo != null) {
                var theObject = (XPBaseObject)memberInfo.GetValue(selectedObject);
                CreateRefElelement(classInfoGraphNode, theObject?.GetType() ?? memberInfo.MemberType, root,theObject, propertyElement,elementSchema);
            }
        }


        public void Export(IEnumerable<XPBaseObject> xpBaseObjects,
                           ISerializationConfigurationGroup serializationConfigurationGroup, string fileName) {
            var document = Export(xpBaseObjects, serializationConfigurationGroup);
            if (fileName != null) {
                var xmlWriterSettings = GetXMLWriterSettings(serializationConfigurationGroup.MinifyOutput);
                using (var textWriter = XmlWriter.Create(new FileStream(fileName, FileMode.Create), xmlWriterSettings)) {
                    document.Save(textWriter);
                    textWriter.Close();
                }
            }
        }

    }

    
    public class ElementSchema {
        private static ElementSchema _defaultSchema;
        private static ElementSchema _minimalSchema;

        public static ElementSchema Get(bool minifyOutput) {
            _defaultSchema =_defaultSchema?? new ElementSchema() {
                SerializedObjectRef = "SerializedObjectRef", Name = "name", IsKey = "isKey", Property = "Property",
                Strategy = "strategy", Type = "type", SerializedObjects = "SerializedObjects",
                SerializedObject = "SerializedObject",Key = "Key"
            };
            _minimalSchema =_minimalSchema?? new ElementSchema {
                IsMinified = true,
                SerializedObjectRef = "a", Name = "b", IsKey = "c", Property = "d",
                Strategy = "e", Type = "f", SerializedObjects = "g",
                SerializedObject = "h", Key = "j"
            };
            return minifyOutput ? _minimalSchema : _defaultSchema;
        }

        private ElementSchema() {
        }

        public string SerializedObjects { get; private set;}
        public string SerializedObject { get;private set; }
        public string Type { get;private set; }
        public string Property { get; private set; }
        public string Name { get; private set; }
        public string IsKey { get; private set; }
        public string SerializedObjectRef { get; private set; }
        public string Strategy { get; private set; }
        public string Key { get; private set; }
        public bool IsMinified { get; private set; }
        public static string Minified => "minified";

        public static ElementSchema Get(XDocument document) {
            var minifyOutput = document.IsMinified();
            return Get(minifyOutput);
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
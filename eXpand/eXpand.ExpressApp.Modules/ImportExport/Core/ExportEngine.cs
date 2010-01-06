using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.ImportExport;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.Core {
    public class ExportEngine {
        public XDocument Export( IEnumerable baseCollection, ISerializationConfiguration serializationConfiguration){
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes = GetSerializedClassInfoGraphNodes(serializationConfiguration);
            xDocument.Add(root);
            foreach (var baseObject in baseCollection) {
                ExportCore((XPBaseObject) baseObject, serializedClassInfoGraphNodes, root);    
            }
            return xDocument;

        }

        void ExportCore(XPBaseObject selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XElement root) {
            var serializedObjectElement = new XElement("SerializedObject");
            serializedObjectElement.Add(new XAttribute("type", selectedObject.GetType().Name));
            root.Add(serializedObjectElement);
            foreach (var classInfoGraphNode in serializedClassInfoGraphNodes) {
                XElement propertyElement = GetPropertyElement(serializedObjectElement, classInfoGraphNode);
                switch (classInfoGraphNode.NodeType) {
                    case NodeType.Simple:
                        propertyElement.Value = GetMemberValue(selectedObject, classInfoGraphNode) + "";
                        break;
                    case NodeType.Object:
                        createObjectProperty(selectedObject, propertyElement, classInfoGraphNode, root);
                        break;
                    case NodeType.Collection:
                        createCollectionProperty(selectedObject, classInfoGraphNode, root, propertyElement);
                        break;
                }
            }
        }

        object GetMemberValue(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode) {
            var memberValue = selectedObject.GetMemberValue(classInfoGraphNode.Name);
            var xpMemberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            if (xpMemberInfo.Converter!= null){
                return xpMemberInfo.Converter.ConvertToStorageType(memberValue);
            }
            return memberValue;
        }

        XElement GetPropertyElement(XElement serializedObjectElement, IClassInfoGraphNode classInfoGraphNode) {
            var propertyElement = new XElement("Property");
            serializedObjectElement.Add(propertyElement);
            propertyElement.Add(new XAttribute("type", classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute("name", classInfoGraphNode.Name));
            propertyElement.Add(new XAttribute("isKey", classInfoGraphNode.Key));
            propertyElement.Add(new XAttribute("isNaturalKey", classInfoGraphNode.NaturalKey));
            return propertyElement;
        }

        void createCollectionProperty(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root,
                                      XElement propertyElement) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObjects = (XPBaseCollection)memberInfo.GetValue(selectedObject);
            foreach (XPBaseObject theObject in theObjects) {
                CreateRefElelement(classInfoGraphNode, root, theObject.GetType().Name, theObject, propertyElement);
            }
        }

        void CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, XElement root, string typeName, XPBaseObject theObject, XElement propertyElement) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            propertyElement.Add(serializedObjectRefElement);
            serializedObjectRefElement.Add(new XAttribute("type",typeName));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes = classInfoGraphNode.Children.OfType<IClassInfoGraphNode>().OrderBy(node => node.NodeType);
            if (theObject != null){
                createRefKeyElements(serializedClassInfoGraphNodes, theObject, serializedObjectRefElement);
                if (serializedObjectRefElement.FindObjectFromRefenceElement(true) == null &&
                    classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                    ExportCore(theObject, serializedClassInfoGraphNodes, root);
            }
        }

        void createRefKeyElements(IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XPBaseObject theObject, XElement serializedObjectRefElement) {
            foreach (var infoGraphNode in serializedClassInfoGraphNodes.Where(node => node.Key)) {
                var serializedObjectRefKeyElement = new XElement("Key");
                serializedObjectRefKeyElement.Add(new XAttribute("name",infoGraphNode.Name));
                serializedObjectRefKeyElement.Value = theObject.GetMemberValue(infoGraphNode.Name).ToString();
                serializedObjectRefElement.Add(serializedObjectRefKeyElement);
            }
        }

        void createObjectProperty(XPBaseObject selectedObject, XElement propertyElement, IClassInfoGraphNode classInfoGraphNode, XElement root) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObject = (XPBaseObject) memberInfo.GetValue(selectedObject);
            string typeName = memberInfo.MemberType.Name;
            CreateRefElelement(classInfoGraphNode, root,typeName,theObject,propertyElement);
        }



        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
            return serializationConfiguration.SerializationGraph[0].Children.OfType<IClassInfoGraphNode>().Where(
                node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize)||node.NodeType!=NodeType.Simple).OrderBy(graphNode => graphNode.NodeType);
        }
    }
}
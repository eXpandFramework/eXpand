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
        public XDocument Export( IList baseCollection, ISerializationConfiguration serializationConfiguration){
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes = GetSerializedClassInfoGraphNodes(serializationConfiguration);
            foreach (var baseObject in baseCollection) {
                ExportCore((XPBaseObject) baseObject, serializedClassInfoGraphNodes, root);    
            }
            xDocument.Add(root);
            return xDocument;

        }

        void ExportCore(XPBaseObject selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XElement root) {
            var serializedObjectElement = new XElement("SerializedObject");
            serializedObjectElement.Add(new XAttribute("type", selectedObject.GetType().Name));
            foreach (var classInfoGraphNode in serializedClassInfoGraphNodes){
                var propertyElement = new XElement("Property");
                serializedObjectElement.Add(propertyElement);
                addCommonAttributes(propertyElement, classInfoGraphNode);
                switch (classInfoGraphNode.NodeType) {
                    case NodeType.Simple:
                        propertyElement.Value = selectedObject.GetMemberValue(classInfoGraphNode.Name) + "";
                        break;
                    case NodeType.Object:
                        createObjectProperty(selectedObject, propertyElement, classInfoGraphNode, root);
                        break;
                    case NodeType.Collection:
                        createCollectionProperty(selectedObject, classInfoGraphNode, root, propertyElement);
                        break;
                }
            }
            root.Add(serializedObjectElement);
        }

        void createCollectionProperty(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root,
                                      XElement propertyElement) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObjects = (XPBaseCollection)memberInfo.GetValue(selectedObject);
            foreach (XPBaseObject theObject in theObjects) {
                XElement refElelement = CreateRefElelement(classInfoGraphNode, root, theObject.GetType().Name, theObject);
                propertyElement.Add(refElelement);                
            }
        }

        XElement CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, XElement root, string typeName, XPBaseObject theObject) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            serializedObjectRefElement.Add(new XAttribute("type",typeName));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes = classInfoGraphNode.Children.OfType<IClassInfoGraphNode>();
            if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject) {
                ExportCore(theObject, serializedClassInfoGraphNodes, root);
            }
            createRefKeyElements(serializedClassInfoGraphNodes, theObject, serializedObjectRefElement);
            return serializedObjectRefElement;
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
            XElement refElelement = CreateRefElelement(classInfoGraphNode, root,typeName,theObject);
            propertyElement.Add(refElelement);
        }

        void addCommonAttributes(XElement propertyElement, IClassInfoGraphNode classInfoGraphNode) {
            propertyElement.Add(new XAttribute("type", classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute("name", classInfoGraphNode.Name));
            propertyElement.Add(new XAttribute("isKey", classInfoGraphNode.Key));
        }


        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
            return serializationConfiguration.SerializationGraph[0].Children.OfType<IClassInfoGraphNode>().Where(
                node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize)||node.NodeType!=NodeType.Simple);
        }
    }
}
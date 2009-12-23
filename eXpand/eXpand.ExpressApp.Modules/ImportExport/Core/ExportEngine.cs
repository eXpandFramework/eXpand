using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.ImportExport;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.Core {
    public static class ExportEngine {

        public static XDocument Export(XPBaseObject selectedObject, ISerializationConfiguration serializationConfiguration)
        {
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes = GetSerializedClassInfoGraphNodes(serializationConfiguration);
            ExportCore(selectedObject, serializedClassInfoGraphNodes, root);
            xDocument.Add(root);
            return xDocument;

        }

        static void ExportCore(XPBaseObject selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XElement root) {
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

        static void createCollectionProperty(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root, XElement propertyElement) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObjects = (XPBaseCollection)memberInfo.GetValue(selectedObject);
            foreach (XPBaseObject theObject in theObjects) {
                object value = theObject.ClassInfo.KeyProperty.GetValue(theObject);
                XElement refElelement = CreateRefElelement(classInfoGraphNode, root, theObject.GetType().Name, value.ToString(), theObject);
                propertyElement.Add(refElelement);                
            }
        }

        static XElement CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, XElement root, string typeName, string value, XPBaseObject theObject) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            serializedObjectRefElement.Add(new XAttribute("type",typeName));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            serializedObjectRefElement.Value =value;
            if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                ExportCore(theObject, classInfoGraphNode.Children.OfType<IClassInfoGraphNode>(), root);
            return serializedObjectRefElement;
        }

        static void createObjectProperty(XPBaseObject selectedObject, XElement propertyElement, IClassInfoGraphNode classInfoGraphNode, XElement root) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObject = (XPBaseObject) memberInfo.GetValue(selectedObject);
            string typeName = memberInfo.MemberType.Name;
            string value = memberInfo.ReferenceType.KeyProperty.GetValue(theObject).ToString();
            XElement refElelement = CreateRefElelement(classInfoGraphNode, root,typeName, value,theObject);
            propertyElement.Add(refElelement);
        }

        static void addCommonAttributes(XElement propertyElement, IClassInfoGraphNode classInfoGraphNode) {
            propertyElement.Add(new XAttribute("type", classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute("name", classInfoGraphNode.Name));
            propertyElement.Add(new XAttribute("isKey", classInfoGraphNode.Key));
        }


        static IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
            return serializationConfiguration.SerializationGraph[0].Children.OfType<IClassInfoGraphNode>().Where(
                node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize)||node.NodeType!=NodeType.Simple);
        }
    }
}
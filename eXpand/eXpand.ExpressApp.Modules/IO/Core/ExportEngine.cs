using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.Core {
    public class ExportEngine {
        readonly Dictionary<XPBaseObject, object> exportedObjecs = new Dictionary<XPBaseObject, object>();
        public XDocument Export(IEnumerable<XPBaseObject> baseCollection) {
            return Export(baseCollection, null);
        }

        public XDocument Export( IEnumerable<XPBaseObject> baseCollection, ISerializationConfiguration serializationConfiguration){
            var xDocument = new XDocument();
            var root = new XElement("SerializedObjects");
            xDocument.Add(root);
            foreach (var baseObject in baseCollection) {
                var serializedClassInfoGraphNodes = GetSerializedClassInfoGraphNodes(baseObject, serializationConfiguration);
                ExportCore(baseObject, serializedClassInfoGraphNodes, root);
            }
            return xDocument;

        }

        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(XPBaseObject baseObject, ISerializationConfiguration serializationConfiguration) {
            ISerializationConfiguration configuration = serializationConfiguration != null?baseObject.GetType() ==serializationConfiguration.TypeToSerialize? serializationConfiguration
                : SerializationConfigurationQuery.Find(baseObject.Session, baseObject.GetType()):GetConfiguration(baseObject.Session,baseObject.GetType());
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        void ExportCore(XPBaseObject selectedObject, IEnumerable<IClassInfoGraphNode> serializedClassInfoGraphNodes, XElement root) {
            if (!(exportedObjecs.ContainsKey(selectedObject))){
                exportedObjecs.Add(selectedObject, null);
                var serializedObjectElement = new XElement("SerializedObject");
                serializedObjectElement.Add(new XAttribute("type", selectedObject.GetType().Name));
                root.Add(serializedObjectElement);
                foreach (var classInfoGraphNode in serializedClassInfoGraphNodes) {
                    XElement propertyElement = GetPropertyElement(serializedObjectElement, classInfoGraphNode);
                    switch (classInfoGraphNode.NodeType) {
                        case NodeType.Simple:
                            SetMemberValue(selectedObject, classInfoGraphNode, propertyElement);
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
        }

        void SetMemberValue(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement propertyElement) {
            var memberValue = selectedObject.GetMemberValue(classInfoGraphNode.Name);
            var xpMemberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            if (xpMemberInfo.Converter!= null) {
                memberValue = (xpMemberInfo.Converter.ConvertToStorageType(memberValue) );
            }
            
            if (memberValue is byte[])
                memberValue = Convert.ToBase64String((byte[]) memberValue);
            if (memberValue is string)
                propertyElement.Add(new XCData(memberValue.ToString()));
            else {
                propertyElement.Value = memberValue+"";
            }
        }

        XElement GetPropertyElement(XElement serializedObjectElement, IClassInfoGraphNode classInfoGraphNode) {
            var propertyElement = new XElement("Property");
            serializedObjectElement.Add(propertyElement);
            propertyElement.Add(new XAttribute("type", classInfoGraphNode.NodeType.ToString().MakeFirstCharLower()));
            propertyElement.Add(new XAttribute("name", classInfoGraphNode.Name));
            propertyElement.Add(new XAttribute("isKey", classInfoGraphNode.Key));
            return propertyElement;
        }

        void createCollectionProperty(XPBaseObject selectedObject, IClassInfoGraphNode classInfoGraphNode, XElement root,
                                      XElement propertyElement) {
            XPMemberInfo memberInfo = selectedObject.ClassInfo.GetMember(classInfoGraphNode.Name);
            var theObjects = (XPBaseCollection)memberInfo.GetValue(selectedObject);
            foreach (XPBaseObject theObject in theObjects) {
                CreateRefElelement(classInfoGraphNode,theObject.GetType().Name, root,  theObject, propertyElement);
            }
        }

        void CreateRefElelement(IClassInfoGraphNode classInfoGraphNode, string typeName, XElement root,  XPBaseObject theObject, XElement propertyElement) {
            var serializedObjectRefElement = new XElement("SerializedObjectRef");
            propertyElement.Add(serializedObjectRefElement);
            serializedObjectRefElement.Add(new XAttribute("type", typeName));
            serializedObjectRefElement.Add(new XAttribute("strategy", classInfoGraphNode.SerializationStrategy));
            if (theObject != null) {
                IEnumerable<IClassInfoGraphNode> classInfoGraphNodes = GetClassInfoGraphNodes(theObject, typeName);
                createRefKeyElements(classInfoGraphNodes, theObject, serializedObjectRefElement);
                if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                    ExportCore(theObject, classInfoGraphNodes, root);
            }
        }

        IEnumerable<IClassInfoGraphNode> GetClassInfoGraphNodes(XPBaseObject theObject, string typeName) {
            var type = ReflectionHelper.GetType(typeName);
            ISerializationConfiguration configuration = GetConfiguration(theObject.Session, type);
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        ISerializationConfiguration GetConfiguration(Session session,  Type type) {
            var serializationConfigurationType = TypesInfo.Instance.SerializationConfigurationType;
            ISerializationConfiguration configuration;
            var findObject = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, serializationConfigurationType,
                                                SerializationConfigurationQuery.GetCriteria(type));
            if (findObject != null)
                configuration =(ISerializationConfiguration)findObject;
            else {
                configuration =
                    (ISerializationConfiguration) Activator.CreateInstance(serializationConfigurationType, session);
                configuration.TypeToSerialize = type;
                new ClassInfoGraphNodeBuilder().Generate(configuration);
            }
            return configuration;
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
            CreateRefElelement(classInfoGraphNode,theObject!= null?theObject.GetType().Name:memberInfo.MemberType.Name, root,  theObject, propertyElement);
        }



        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
            return ((IEnumerable)((XPBaseObject)serializationConfiguration).GetMemberValue(serializationConfiguration.GetPropertyName(x => x.SerializationGraph))).
                OfType<IClassInfoGraphNode>().Where(
                node =>(node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize) ||
                node.NodeType != NodeType.Simple).OrderBy(graphNode => graphNode.NodeType);
        }
    }
}
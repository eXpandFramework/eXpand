using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;
using System.Linq;
using TypesInfo = eXpand.ExpressApp.IO.Core.TypesInfo;

namespace eXpand.ExpressApp.IO.PersistentTypesHelpers {


    public class ClassInfoGraphNodeBuilder{
        string[] _excludedMembers;

        public string[] ExcludedMembers{
            get { return _excludedMembers; }
        }

        public void Generate(ISerializationConfiguration configuration){
            Type typeToSerialize = configuration.TypeToSerialize;
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(configuration);
            var node =(IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            node.Name = typeToSerialize.Name;
            configuration.SerializationGraph.Add(node);
            foreach (IClassInfoGraphNode classInfoGraphNode in GetGraph(objectSpace, XafTypesInfo.CastTypeToTypeInfo(typeToSerialize),null)) {
                node.Children.Add(classInfoGraphNode);
            }
        }

        IEnumerable<IClassInfoGraphNode> GetGraph(ObjectSpace objectSpace, ITypeInfo typeToSerialize, ITypeInfo excludeSerialize) {
            var classInfoGraphNodes = new List<IClassInfoGraphNode>();
            IEnumerable<IMemberInfo> memberInfos = GetMemberInfos(typeToSerialize);
            foreach (var memberInfo in memberInfos) {
                if (memberInfo.MemberTypeInfo.IsPersistent){
                    addRefNodes(memberInfo, excludeSerialize, objectSpace, classInfoGraphNodes);
                }
                else if (memberInfo.IsList){
                    addListNodes(typeToSerialize, memberInfo, excludeSerialize, objectSpace, classInfoGraphNodes);
                }
                else {
                    IClassInfoGraphNode addClassInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes,NodeType.Simple);
                    addClassInfoGraphNode.Key = memberInfo.IsKey;
                }
            }

            return classInfoGraphNodes;
        }

        void addListNodes(ITypeInfo typeToSerialize, IMemberInfo memberInfo, ITypeInfo excludeSerialize, ObjectSpace objectSpace, List<IClassInfoGraphNode> classInfoGraphNodes) {
            if (isNotSerialized(memberInfo.ListElementTypeInfo, excludeSerialize)){
                IClassInfoGraphNode classInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes,NodeType.Collection);
                classInfoGraphNode.SerializationStrategy=SerializationStrategy.SerializeAsObject;
                if (!isSelfReference(memberInfo,memberInfo.ListElementTypeInfo)){
                    IEnumerable<IClassInfoGraphNode> infoGraphNodes = GetGraph(objectSpace, memberInfo.ListElementTypeInfo, typeToSerialize);
                    foreach (var infoGraphNode in infoGraphNodes){
                        classInfoGraphNode.Children.Add(infoGraphNode);
                    }
                }
                else
                    classInfoGraphNode.SerializationStrategy=SerializationStrategy.DoNotSerialize;
            }
        }

        void addRefNodes(IMemberInfo memberInfo, ITypeInfo excludeSerialize, ObjectSpace objectSpace, List<IClassInfoGraphNode> classInfoGraphNodes) {
            if (isNotSerialized(memberInfo.MemberTypeInfo, excludeSerialize))
            {
                IClassInfoGraphNode classInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes,NodeType.Object);
                classInfoGraphNode.SerializationStrategy=SerializationStrategy.SerializeAsObject;
                if (!isSelfReference(memberInfo, memberInfo.MemberTypeInfo)){
                    IEnumerable<IClassInfoGraphNode> infoGraphNodes = GetGraph(objectSpace, memberInfo.MemberTypeInfo, excludeSerialize);
                    foreach (var infoGraphNode in infoGraphNodes){
                        classInfoGraphNode.Children.Add(infoGraphNode);
                    }
                }
                else
                    classInfoGraphNode.SerializationStrategy=SerializationStrategy.DoNotSerialize;
            }
        }

        bool isSelfReference(IMemberInfo memberInfo, ITypeInfo typeInfo) {
            return memberInfo.Owner == typeInfo;
        }

        IClassInfoGraphNode AddClassInfoGraphNode(ObjectSpace objectSpace, IMemberInfo memberInfo, List<IClassInfoGraphNode> classInfoGraphNodes, NodeType nodeType) {
            var classInfoGraphNode =
                (IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNode.NodeType=nodeType;            
            classInfoGraphNodes.Add(classInfoGraphNode);
            return classInfoGraphNode;
        }

        bool isNotSerialized(ITypeInfo typeInfo, ITypeInfo excludeSerialize) {
            return typeInfo!=excludeSerialize;
        }


        IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo typeInfo){
            _excludedMembers = new[] { XPObject.Fields.GCRecord.PropertyName, XPObject.Fields.OptimisticLockField.PropertyName, XPObject.Fields.ObjectType.PropertyName};
            return typeInfo.Members.Where(info => isPersistent(info) && !(_excludedMembers.Contains(info.Name)));
        }

        bool isPersistent(IMemberInfo info) {
            return (info.IsPersistent || (info.IsList && info.ListElementTypeInfo.IsPersistent));
        }
    }
}
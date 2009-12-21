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
                else
                    AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes);
            }

            return classInfoGraphNodes;
        }

        void addListNodes(ITypeInfo typeToSerialize, IMemberInfo memberInfo, ITypeInfo excludeSerialize, ObjectSpace objectSpace, List<IClassInfoGraphNode> classInfoGraphNodes) {
            if (isNotSerialized(memberInfo.ListElementTypeInfo, excludeSerialize)){
                IClassInfoGraphNode classInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes);
                IEnumerable<IClassInfoGraphNode> infoGraphNodes = GetGraph(objectSpace, memberInfo.ListElementTypeInfo, typeToSerialize);
                foreach (var infoGraphNode in infoGraphNodes){
                    classInfoGraphNode.Children.Add(infoGraphNode);
                }
            }
        }

        void addRefNodes(IMemberInfo memberInfo, ITypeInfo excludeSerialize, ObjectSpace objectSpace, List<IClassInfoGraphNode> classInfoGraphNodes) {
            if (isNotSerialized(memberInfo.MemberTypeInfo, excludeSerialize)){
                IClassInfoGraphNode classInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes);
                classInfoGraphNode.SerializationStrategy=SerializationStrategy.SerializeAsObject;
                IEnumerable<IClassInfoGraphNode> infoGraphNodes = GetGraph(objectSpace, memberInfo.MemberTypeInfo, excludeSerialize);
                foreach (var infoGraphNode in infoGraphNodes){
                    classInfoGraphNode.Children.Add(infoGraphNode);
                }
            }
        }

        IClassInfoGraphNode AddClassInfoGraphNode(ObjectSpace objectSpace, IMemberInfo memberInfo, List<IClassInfoGraphNode> classInfoGraphNodes) {
            var classInfoGraphNode =
                (IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNodes.Add(classInfoGraphNode);
            return classInfoGraphNode;
        }

        bool isNotSerialized(ITypeInfo typeInfo, ITypeInfo excludeSerialize) {
            return typeInfo!=excludeSerialize;
        }


        IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo typeInfo){
            _excludedMembers = new[] { XPObject.Fields.GCRecord.PropertyName, XPObject.Fields.OptimisticLockField.PropertyName };
            return typeInfo.Members.Where(info => isPersistent(info) && !(_excludedMembers.Contains(info.Name)));
        }

        bool isPersistent(IMemberInfo info) {
            return (info.IsPersistent || (info.IsList && info.ListElementTypeInfo.IsPersistent));
        }
    }
}
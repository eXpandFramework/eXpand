using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
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

        public void Generate(ISerializationConfiguration serializationConfiguration){
            var typeToSerialize = serializationConfiguration.TypeToSerialize;
            var castTypeToTypeInfo = XafTypesInfo.CastTypeToTypeInfo(typeToSerialize);
            var objectSpace = ObjectSpace.FindObjectSpace(serializationConfiguration);
            foreach (var descendant in ReflectionHelper.FindTypeDescendants(castTypeToTypeInfo)) {
                generate(objectSpace, descendant.Type);
            }
            foreach (IClassInfoGraphNode classInfoGraphNode in createGraph(objectSpace, castTypeToTypeInfo)){
                serializationConfiguration.SerializationGraph.Add(classInfoGraphNode);
            }
            
        }

        IEnumerable<IClassInfoGraphNode> createGraph(ObjectSpace objectSpace, ITypeInfo typeToSerialize){
            IEnumerable<IMemberInfo> memberInfos = getMemberInfos(typeToSerialize);
            return memberInfos.Select(memberInfo => (!memberInfo.MemberTypeInfo.IsPersistent && !memberInfo.IsList)||memberInfo.MemberType==typeof(byte[])
                                                        ? addSimpleNode(memberInfo, objectSpace)
                                                        : addComplexNode(memberInfo, objectSpace));
        }

        Type getSerializedType(IMemberInfo memberInfo) {
            return memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType;
        }

        IClassInfoGraphNode addComplexNode(IMemberInfo memberInfo, ObjectSpace objectSpace) {
            NodeType nodeType=memberInfo.MemberTypeInfo.IsPersistent?NodeType.Object : NodeType.Collection;
            IClassInfoGraphNode classInfoGraphNode = addClassInfoGraphNode(objectSpace, memberInfo,nodeType);
            classInfoGraphNode.SerializationStrategy = SerializationStrategy.SerializeAsObject;
            generate(objectSpace, ReflectionHelper.GetType(classInfoGraphNode.TypeName));
            return classInfoGraphNode;

        }

        void generate(ObjectSpace objectSpace, Type typeToSerialize) {
            if (!SerializationConfigurationQuery.ConfigurationExists(objectSpace.Session, typeToSerialize))
            {
                var serializationConfiguration =
                    (ISerializationConfiguration)
                    objectSpace.CreateObject(TypesInfo.Instance.SerializationConfigurationType);
                serializationConfiguration.TypeToSerialize = typeToSerialize;                
                Generate(serializationConfiguration);
            }
        }

        

        IClassInfoGraphNode addSimpleNode(IMemberInfo memberInfo, ObjectSpace objectSpace) {
            IClassInfoGraphNode addClassInfoGraphNode = this.addClassInfoGraphNode(objectSpace, memberInfo, NodeType.Simple);
            addClassInfoGraphNode.Key = memberInfo.IsKey;
            return addClassInfoGraphNode;
        }

        IClassInfoGraphNode addClassInfoGraphNode(ObjectSpace objectSpace, IMemberInfo memberInfo, NodeType nodeType) {
            var classInfoGraphNode =(IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNode.TypeName = getSerializedType(memberInfo).Name;
            classInfoGraphNode.NodeType=nodeType;            
            return classInfoGraphNode;
        }

        IEnumerable<IMemberInfo> getMemberInfos(ITypeInfo typeInfo){
            _excludedMembers = new[] {
                                         XPObject.Fields.GCRecord.PropertyName,
                                         XPObject.Fields.OptimisticLockField.PropertyName,
                                         XPObject.Fields.ObjectType.PropertyName
                                     };
            return typeInfo.Members.Where(info => isPersistent(info) && !(_excludedMembers.Contains(info.Name)));
        }

        bool isPersistent(IMemberInfo info) {
            return (info.IsPersistent || (info.IsList && info.ListElementTypeInfo.IsPersistent));
        }
    }
}
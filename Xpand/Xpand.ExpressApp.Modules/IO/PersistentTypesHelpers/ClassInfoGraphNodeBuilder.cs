using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Linq;
using Xpand.Persistent.Base.ImportExport;
using TypesInfo = Xpand.ExpressApp.IO.Core.TypesInfo;

namespace Xpand.ExpressApp.IO.PersistentTypesHelpers {
    public class ClassInfoGraphNodeBuilder{
        string[] _excludedMembers;
        ISerializationConfigurationGroup _serializationConfigurationGroup;

        public string[] ExcludedMembers{
            get { return _excludedMembers; }
        }

        public void Generate(ISerializationConfiguration serializationConfiguration){
            var typeToSerialize = serializationConfiguration.TypeToSerialize;
            var castTypeToTypeInfo = XafTypesInfo.CastTypeToTypeInfo(typeToSerialize);
            var objectSpace = ObjectSpace.FindObjectSpace(serializationConfiguration);
            _serializationConfigurationGroup = serializationConfiguration.SerializationConfigurationGroup;
            if (_serializationConfigurationGroup== null)
                throw new NullReferenceException("_serializationConfigurationGroup");
            foreach (var descendant in ReflectionHelper.FindTypeDescendants(castTypeToTypeInfo)) {
                Generate(objectSpace, descendant.Type);
            }
            foreach (IClassInfoGraphNode classInfoGraphNode in CreateGraph(objectSpace, castTypeToTypeInfo)){
                serializationConfiguration.SerializationGraph.Add(classInfoGraphNode);
            }
            
        }

        IEnumerable<IClassInfoGraphNode> CreateGraph(ObjectSpace objectSpace, ITypeInfo typeToSerialize){
            IEnumerable<IMemberInfo> memberInfos = GetMemberInfos(typeToSerialize);
            return memberInfos.Select(memberInfo => (!memberInfo.MemberTypeInfo.IsPersistent && !memberInfo.IsList)||memberInfo.MemberType==typeof(byte[])
                                                        ? AddSimpleNode(memberInfo, objectSpace)
                                                        : AddComplexNode(memberInfo, objectSpace));
        }

        Type GetSerializedType(IMemberInfo memberInfo) {
            return memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType;
        }

        IClassInfoGraphNode AddComplexNode(IMemberInfo memberInfo, ObjectSpace objectSpace) {
            NodeType nodeType=memberInfo.MemberTypeInfo.IsPersistent?NodeType.Object : NodeType.Collection;
            IClassInfoGraphNode classInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo,nodeType);
            classInfoGraphNode.SerializationStrategy = SerializationStrategy.SerializeAsObject;
            Generate(objectSpace, ReflectionHelper.GetType(classInfoGraphNode.TypeName));
            return classInfoGraphNode;

        }

        void Generate(ObjectSpace objectSpace, Type typeToSerialize) {
            if (!SerializationConfigurationQuery.ConfigurationExists(objectSpace.Session, typeToSerialize,_serializationConfigurationGroup)){
                var serializationConfiguration =
                    (ISerializationConfiguration)
                    objectSpace.CreateObject(TypesInfo.Instance.SerializationConfigurationType);
                serializationConfiguration.SerializationConfigurationGroup=_serializationConfigurationGroup;
                serializationConfiguration.TypeToSerialize = typeToSerialize;                
                Generate(serializationConfiguration);
            }
        }

        

        IClassInfoGraphNode AddSimpleNode(IMemberInfo memberInfo, ObjectSpace objectSpace) {
            IClassInfoGraphNode addClassInfoGraphNode = AddClassInfoGraphNode(objectSpace, memberInfo, NodeType.Simple);
            addClassInfoGraphNode.Key = memberInfo.IsKey;
            return addClassInfoGraphNode;
        }

        IClassInfoGraphNode AddClassInfoGraphNode(ObjectSpace objectSpace, IMemberInfo memberInfo, NodeType nodeType) {
            var classInfoGraphNode =(IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNode.TypeName = GetSerializedType(memberInfo).Name;
            classInfoGraphNode.NodeType=nodeType;            
            return classInfoGraphNode;
        }

        IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo typeInfo){
            _excludedMembers = new[] {
                                         XPObject.Fields.OptimisticLockField.PropertyName,
                                         XPObject.Fields.ObjectType.PropertyName
                                     };
            return typeInfo.Members.Where(info => IsPersistent(info) && !(_excludedMembers.Contains(info.Name)));
        }

        bool IsPersistent(IMemberInfo info) {
            return (info.IsPersistent || (info.IsList && info.ListElementTypeInfo.IsPersistent));
        }
    }
}
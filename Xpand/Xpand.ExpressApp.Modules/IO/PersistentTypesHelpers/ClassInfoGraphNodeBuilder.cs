using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Xpo;
using TypesInfo = Xpand.ExpressApp.IO.Core.TypesInfo;

namespace Xpand.ExpressApp.IO.PersistentTypesHelpers {
    public class ClassInfoGraphNodeBuilder {
        string[] _excludedMembers;
        ISerializationConfigurationGroup _serializationConfigurationGroup;

        public string[] ExcludedMembers {
            get { return _excludedMembers; }
        }

        public void Generate(ISerializationConfiguration serializationConfiguration) {
            var typeToSerialize = serializationConfiguration.TypeToSerialize;
            var castTypeToTypeInfo = XafTypesInfo.CastTypeToTypeInfo(typeToSerialize);
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(serializationConfiguration);
            _serializationConfigurationGroup = serializationConfiguration.SerializationConfigurationGroup;
            if (_serializationConfigurationGroup == null)
                throw new NullReferenceException("_serializationConfigurationGroup");
            foreach (var descendant in ReflectionHelper.FindTypeDescendants(castTypeToTypeInfo)) {
                Generate(objectSpace, descendant.Type);
            }
            foreach (IClassInfoGraphNode classInfoGraphNode in CreateGraph(objectSpace, castTypeToTypeInfo)) {
                serializationConfiguration.SerializationGraph.Add(classInfoGraphNode);
            }

        }

        IEnumerable<IClassInfoGraphNode> CreateGraph(IObjectSpace objectSpace, ITypeInfo typeToSerialize) {
            var memberInfos = GetMemberInfos(typeToSerialize);
            var classInfoGraphNodes = CreateGraphCore(memberInfos, objectSpace).ToArray();
            return classInfoGraphNodes;
        }
        
        IEnumerable<IClassInfoGraphNode> CreateGraphCore(IEnumerable<IMemberInfo> memberInfos, IObjectSpace objectSpace) {
            return memberInfos.Select(memberInfo => (!memberInfo.MemberTypeInfo.IsPersistent && !memberInfo.IsList) || memberInfo.MemberType == typeof(byte[])
                                                                                    ? CreateSimpleNode(memberInfo, objectSpace)
                                                                                    : CreateComplexNode(memberInfo, objectSpace)).ToList();
        }

        Type GetSerializedType(IMemberInfo memberInfo) {
            return memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType;
        }

        IClassInfoGraphNode CreateComplexNode(IMemberInfo memberInfo, IObjectSpace objectSpace) {
            NodeType nodeType = memberInfo.MemberTypeInfo.IsPersistent ? NodeType.Object : NodeType.Collection;
            IClassInfoGraphNode classInfoGraphNode = CreateClassInfoGraphNode(objectSpace, memberInfo, nodeType);
            classInfoGraphNode.SerializationStrategy = GetSerializationStrategy(memberInfo, SerializationStrategy.SerializeAsObject);
            if (classInfoGraphNode.SerializationStrategy == SerializationStrategy.SerializeAsObject)
                Generate(objectSpace, ReflectionHelper.GetType(classInfoGraphNode.TypeName));
            return classInfoGraphNode;

        }

        void Generate(IObjectSpace objectSpace, Type typeToSerialize) {
            if (!SerializationConfigurationQuery.ConfigurationExists(((XPObjectSpace)objectSpace).Session, typeToSerialize, _serializationConfigurationGroup)) {
                var serializationConfiguration =
                    (ISerializationConfiguration)
                    objectSpace.CreateObject(TypesInfo.Instance.SerializationConfigurationType);
                serializationConfiguration.SerializationConfigurationGroup = _serializationConfigurationGroup;
                serializationConfiguration.TypeToSerialize = typeToSerialize;
                Generate(serializationConfiguration);
            }
        }



        IClassInfoGraphNode CreateSimpleNode(IMemberInfo memberInfo, IObjectSpace objectSpace) {
            return CreateClassInfoGraphNode(objectSpace, memberInfo, NodeType.Simple);
        }

        bool IsKey(IMemberInfo memberInfo) {
            return memberInfo.IsKey || memberInfo.FindAttribute<SerializationKeyAttribute>() != null;
        }


        IClassInfoGraphNode CreateClassInfoGraphNode(IObjectSpace objectSpace, IMemberInfo memberInfo, NodeType nodeType) {
            var classInfoGraphNode = (IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNode.SerializationStrategy = GetSerializationStrategy(memberInfo, classInfoGraphNode.SerializationStrategy);
            classInfoGraphNode.TypeName = GetSerializedType(memberInfo).Name;
            classInfoGraphNode.NodeType = nodeType;
            if (classInfoGraphNode.SerializationStrategy != SerializationStrategy.DoNotSerialize)
                classInfoGraphNode.Key = IsKey(memberInfo);
            return classInfoGraphNode;
        }

        SerializationStrategy GetSerializationStrategy(IMemberInfo memberInfo, SerializationStrategy serializationStrategy) {
            if (memberInfo.MemberTypeInfo.IsPersistent) {
                var attribute = memberInfo.MemberTypeInfo.FindAttribute<SerializationStrategyAttribute>();
                if (attribute != null) {
                    return attribute.SerializationStrategy;
                }
            }
            var serializationStrategyAttribute = memberInfo.FindAttribute<SerializationStrategyAttribute>();
            return serializationStrategyAttribute != null ? serializationStrategyAttribute.SerializationStrategy : serializationStrategy;
        }

        IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo typeInfo) {
            _excludedMembers = new[] {
                                         XPObject.Fields.OptimisticLockField.PropertyName,
                                         XPObject.Fields.ObjectType.PropertyName,
                                         XpandCustomObject.ChangedPropertiesName
                                     };
            return typeInfo.Members.Where(info => IsPersistent(info) && !(_excludedMembers.Contains(info.Name))).OrderByDescending(memberInfo => memberInfo.IsKey);
        }

        bool IsPersistent(IMemberInfo info) {
            return (info.IsPersistent || (info.IsList && info.ListElementTypeInfo != null && info.ListElementTypeInfo.IsPersistent));
        }

        public void ApplyStrategy(SerializationStrategy serializationStrategy, ISerializationConfiguration serializationConfiguration) {
            var serializationConfigurationGroup = serializationConfiguration.SerializationConfigurationGroup;
            var infoGraphNodes = serializationConfigurationGroup.Configurations.SelectMany(configuration => configuration.SerializationGraph);
            var classInfoGraphNodes = infoGraphNodes.Where(node => node.TypeName == serializationConfiguration.TypeToSerialize.Name);
            foreach (var classInfoGraphNode in classInfoGraphNodes) {
                classInfoGraphNode.SerializationStrategy = serializationStrategy;
            }
        }
    }
}
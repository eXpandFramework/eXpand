using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;
using TypesInfo = eXpand.ExpressApp.ImportExport.Core.TypesInfo;
using System.Linq;

namespace eXpand.ExpressApp.ImportExport.PersistentTypesHelpers
{
    public class ClassInfoGraphNodeBuilder
    {
        string[] _excludedMembers;

        public string[] ExcludedMembers
        {
            get { return _excludedMembers; }
        }

        public void Generate(ISerializationConfiguration configuration)
        {
            Type typeToSerialize = configuration.TypeToSerialize;
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(configuration);
            foreach (IClassInfoGraphNode classInfoGraphNode in GetGraph(objectSpace, typeToSerialize, null)) {
                configuration.SerializationGraph.Add(classInfoGraphNode);
            }
        }

        IEnumerable<IClassInfoGraphNode> GetGraph(ObjectSpace objectSpace, Type typeToSerialize, Action<IList<IMemberInfo>> action)
        {
            var classInfoGraphNodes = new List<IClassInfoGraphNode>();

            IList<IMemberInfo> memberInfos = GetMemberInfos(typeToSerialize);
            if (action != null)
                action.Invoke(memberInfos);
            foreach (IMemberInfo memberInfo in memberInfos)
            {
                var classInfoGraphNode =
                    (IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
                classInfoGraphNode.Name = memberInfo.Name;
                classInfoGraphNodes.Add(classInfoGraphNode);
                if (memberInfo.MemberTypeInfo.IsPersistent)
                    Generate(classInfoGraphNode, memberInfo.MemberTypeInfo, objectSpace, action);
                else if (memberInfo.MemberTypeInfo.IsListType)
                {
                    string name = memberInfo.Owner.Name;
                    Generate(classInfoGraphNode, memberInfo.ListElementTypeInfo, objectSpace, infos => infos.Remove(infos.Where(info => info.Name == name).Single()));
                }
            }
            return classInfoGraphNodes;
        }

        void Generate(IClassInfoGraphNode classInfoGraphNode, ITypeInfo memberInfo, ObjectSpace objectSpace, Action<IList<IMemberInfo>> action)
        {
            foreach (var infoGraphNode in GetGraph(objectSpace, memberInfo.Type, action))
            {
                classInfoGraphNode.Children.Add(infoGraphNode);
            }
        }

        IList<IMemberInfo> GetMemberInfos(Type typeToSerialize)
        {
            _excludedMembers = new[] { XPObject.Fields.GCRecord.PropertyName, XPObject.Fields.OptimisticLockField.PropertyName };
            return XafTypesInfo.Instance.FindTypeInfo(typeToSerialize).Members.Where(info => (info.IsPersistent || (info.IsList && info.ListElementTypeInfo.IsPersistent)) && !(_excludedMembers.Contains(info.Name))).ToList();
        }
    }

    public static class SerializationConfigurationQuery
    {
        public static CriteriaOperator GetCriteria(Type serializationConfigurationType)
        {
            const ISerializationConfiguration serializationConfiguration = null;
            return new BinaryOperator(serializationConfiguration.GetPropertyName(x => x.TypeToSerialize),
                                      serializationConfigurationType);
        }
    }
}

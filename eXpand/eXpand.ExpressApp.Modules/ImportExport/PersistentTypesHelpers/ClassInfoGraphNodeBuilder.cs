using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
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

        public void Generate(ISerializationConfiguration configuration){
            Type typeToSerialize = configuration.TypeToSerialize;
            ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(configuration);
            var node =(IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            node.Name = typeToSerialize.Name;
            node.TypeName = typeToSerialize.Name;
            configuration.SerializationGraph.Add(node);
            createGraph(objectSpace, XafTypesInfo.CastTypeToTypeInfo(typeToSerialize), node);
//            cloneNodes(objectSpace);
        }

        void cloneNodes(ObjectSpace objectSpace) {
            var classInfoGraphNodes = new XPCollection(PersistentCriteriaEvaluationBehavior.InTransaction, objectSpace.Session,TypesInfo.Instance.ClassInfoGraphNodeType,
                                                       null).OfType<IClassInfoGraphNode>();
            var infoGraphNodesWithChildren = classInfoGraphNodes.Where(graphNode => graphNode.Children.Count > 0 && !(string.IsNullOrEmpty(graphNode.TypeName))).ToList();
            IEnumerable<IClassInfoGraphNode> nodesWithoutChildren = GetNodesWithoutChildren(objectSpace, infoGraphNodesWithChildren);

            foreach (IClassInfoGraphNode classInfoGraphNode in infoGraphNodesWithChildren) {
                IClassInfoGraphNode infoGraphNode = classInfoGraphNode;
                var infoGraphNodes = nodesWithoutChildren.Where(graphNode => graphNode.TypeName==infoGraphNode.TypeName).ToList();
                for (int i = infoGraphNodes.Count()-1; i >-1; i--) {
                    var graphNode = infoGraphNodes[i];
                    for (int index = classInfoGraphNode.Children.Count-1; index >-1; index--){
                        var child = classInfoGraphNode.Children[index];
                        var cloneTo =(IClassInfoGraphNode)new Cloner().CloneTo((IXPSimpleObject) child, TypesInfo.Instance.ClassInfoGraphNodeType);
                        graphNode.Children.Add(cloneTo);
                    }
                }
            }
        }

        IEnumerable<IClassInfoGraphNode> GetNodesWithoutChildren(ObjectSpace objectSpace, IEnumerable<IClassInfoGraphNode> infoGraphNodesWithChildren) {
            var graphNodes = new XPCollection(PersistentCriteriaEvaluationBehavior.InTransaction, objectSpace.Session, TypesInfo.Instance.ClassInfoGraphNodeType,
                                              null);
            return graphNodes.OfType<IClassInfoGraphNode>().Where(graphNode =>
                                                                  (infoGraphNodesWithChildren.Select(infoGraphNode => infoGraphNode.TypeName).Contains(graphNode.TypeName)) &&
                                                                  graphNode.Children.Count == 0).ToList();
        }

        void createGraph(ObjectSpace objectSpace, ITypeInfo typeToSerialize, IClassInfoGraphNode classInfoGraphNode) {
            IEnumerable<IMemberInfo> memberInfos = getMemberInfos(typeToSerialize);
            foreach (IMemberInfo memberInfo in memberInfos) {
                if (!memberInfo.MemberTypeInfo.IsPersistent && !memberInfo.IsList)
                    addSimpleNode(memberInfo, classInfoGraphNode.Children, objectSpace);
                else{
                    addComplexNode(memberInfo, objectSpace, classInfoGraphNode.Children);
                }
            }
        }

        Type getSerializedType(IMemberInfo memberInfo) {
            return memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType;
        }


        bool isSelfReference(IMemberInfo memberInfo) {
            return memberInfo.Owner == XafTypesInfo.CastTypeToTypeInfo(getSerializedType(memberInfo));
        }

        void addComplexNode(IMemberInfo memberInfo, ObjectSpace objectSpace,IBindingList classInfoGraphNodes) {
            NodeType nodeType=memberInfo.MemberTypeInfo.IsPersistent?NodeType.Object : NodeType.Collection;
            IClassInfoGraphNode classInfoGraphNode = addClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes,nodeType);
            
            ITypeInfo typeToSerialize = XafTypesInfo.CastTypeToTypeInfo(getSerializedType(memberInfo));
            if (!isSelfReference(memberInfo)&&!isSerialized(objectSpace,typeToSerialize)) {
                createGraph(objectSpace, typeToSerialize, classInfoGraphNode);
                classInfoGraphNode.SerializationStrategy = SerializationStrategy.SerializeAsObject;
            }
            else {
                classInfoGraphNode.SerializationStrategy = SerializationStrategy.SerializeAsValue;
            }
        }

        bool isSerialized(ObjectSpace objectSpace, ITypeInfo typeToSerialize) {
            
            var classInfoGraphNode =(IClassInfoGraphNode)objectSpace.Session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                               TypesInfo.Instance.ClassInfoGraphNodeType,
                                               new BinaryOperator("TypeName", typeToSerialize.Name));
            var serialized = classInfoGraphNode!= null&& classInfoGraphNode.Children.Count > 0;
            return serialized;
        }


        void addSimpleNode(IMemberInfo memberInfo, IBindingList classInfoGraphNodes, ObjectSpace objectSpace) {
            IClassInfoGraphNode addClassInfoGraphNode = this.addClassInfoGraphNode(objectSpace, memberInfo, classInfoGraphNodes, NodeType.Simple);
            addClassInfoGraphNode.Key = memberInfo.IsKey;
            addClassInfoGraphNode.NaturalKey = memberInfo.IsKey;
        }

        IClassInfoGraphNode addClassInfoGraphNode(ObjectSpace objectSpace, IMemberInfo memberInfo, IBindingList classInfoGraphNodes, NodeType nodeType) {
            var classInfoGraphNode =(IClassInfoGraphNode)objectSpace.CreateObject(TypesInfo.Instance.ClassInfoGraphNodeType);
            classInfoGraphNode.Name = memberInfo.Name;
            classInfoGraphNode.TypeName = getSerializedType(memberInfo).Name;
            classInfoGraphNode.NodeType=nodeType;            
            classInfoGraphNodes.Add(classInfoGraphNode);
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
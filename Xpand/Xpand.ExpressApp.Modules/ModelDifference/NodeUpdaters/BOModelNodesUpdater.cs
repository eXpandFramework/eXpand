using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.Xpo.MetaData;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.ModelDifference.NodeUpdaters {
    public class BOModelMemberNodesUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelBoModelClassMembers = ((IModelBOModelClassMembers)node);
            var xpClassInfo = XpandModuleBase.Dictiorary.QueryClassInfo(((IModelClass)modelBoModelClassMembers.Parent).TypeInfo.Type);
            if (xpClassInfo == null) return;
            for (int index = modelBoModelClassMembers.Count - 1; index > -1; index--) {
                var modelClassMember = modelBoModelClassMembers[index];
                var xpandCustomMemberInfo = xpClassInfo.FindMember(modelClassMember.Name) as XpandCustomMemberInfo;
                if (xpandCustomMemberInfo != null) {
                    modelBoModelClassMembers.Remove(modelClassMember);
                    var propertyNode = GetPropertyNode(xpandCustomMemberInfo, modelClassMember.ModelClass);
                    CreateMemberNode(modelClassMember.ModelClass, modelClassMember.MemberInfo, propertyNode);
                }
            }
        }

        IModelMember GetPropertyNode(XPCustomMemberInfo memberInfo, IModelClass modelClass) {
            if (memberInfo is XpandCalcMemberInfo) {
                return GetCalcNode(memberInfo, modelClass);
            }
            if (memberInfo is XpandCollectionMemberInfo) {
                return GetCollectionNode(memberInfo, modelClass);
            }
            if (!(memberInfo.IsPersistent))
                return modelClass.OwnMembers.AddNode<IModelRuntimeNonPersistentMember>(memberInfo.Name);
            if (memberInfo is XpandCustomMemberInfo)
                return modelClass.OwnMembers.AddNode<IModelRuntimeMember>(memberInfo.Name);
            throw new NotImplementedException(memberInfo.ToString());
        }

        IModelMember GetCollectionNode(XPCustomMemberInfo memberInfo, IModelClass modelClass) {
            var modelRuntimeOrphanedColection = modelClass.OwnMembers.AddNode<IModelRuntimeOrphanedColection>(memberInfo.Name);
            modelRuntimeOrphanedColection.CollectionType =modelClass.Application.BOModel[memberInfo.MemberType.GetGenericArguments()[0].FullName];
            modelRuntimeOrphanedColection.Criteria = ((XpandCollectionMemberInfo) memberInfo).Criteria;
            return modelRuntimeOrphanedColection;
        }

        IModelMember GetCalcNode(XPCustomMemberInfo memberInfo, IModelClass modelClass) {
            var modelRuntimeCalculatedMember = modelClass.OwnMembers.AddNode<IModelRuntimeCalculatedMember>(memberInfo.Name);
            modelRuntimeCalculatedMember.AliasExpression =((PersistentAliasAttribute) memberInfo.FindAttributeInfo(typeof (PersistentAliasAttribute))).AliasExpression;
            return modelRuntimeCalculatedMember;
        }

        protected virtual void CreateMemberNode(IModelClass classNode, IMemberInfo memberInfo, IModelMember propertyNode) {

            propertyNode.SetValue("MemberInfo", memberInfo);
            var indexAttribute = memberInfo.FindAttribute<IndexAttribute>();
            if (indexAttribute != null) {
                propertyNode.Index = (int)indexAttribute.Value;
            }
            const BindingFlags bindingFlags = BindingFlags.NonPublic|BindingFlags.Instance;
            var methodInfo = typeof(ModelNode).GetMethod("SetSerializedValue",bindingFlags);
            foreach (CustomAttribute attribute in memberInfo.FindAttributes<CustomAttribute>()) {
                methodInfo.Invoke(propertyNode, new object[] {attribute.Name, attribute.Value});
            }
            var exportedAttributeValues = new Dictionary<string, object>();
            foreach (ModelExportedValuesAttribute attribute in memberInfo.FindAttributes<ModelExportedValuesAttribute>()) {
                attribute.FillValues(exportedAttributeValues);
            }
            methodInfo = typeof(ModelNode).GetMethod("SetSerializableValues", bindingFlags);
            methodInfo.Invoke(propertyNode, new object[] { exportedAttributeValues });
        }

    }
    public class BOModelNodesUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {


        public override void UpdateNode(ModelNode node) {
            var boModel = ((IModelBOModel)node);
            var classNode = boModel[typeof(RoleModelDifferenceObject).FullName];
            if (SecuritySystem.UserType != null && !(SecuritySystem.Instance is ISecurityComplex) && classNode != null) {
                node.Remove((ModelNode)classNode);
            }
        }
    }
}
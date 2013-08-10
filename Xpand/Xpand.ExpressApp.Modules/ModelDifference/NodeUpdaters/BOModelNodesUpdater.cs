using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.ModelDifference.NodeUpdaters {
    public class BOModelMemberNodesUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            if (!XpandModuleBase.IsLoadingExternalModel())
                return;
            var modelBoModelClassMembers = ((IModelBOModelClassMembers)node);
            var xpClassInfo = XpandModuleBase.Dictiorary.QueryClassInfo(((IModelClass)modelBoModelClassMembers.Parent).TypeInfo.Type);
            if (xpClassInfo == null) return;
            for (int index = modelBoModelClassMembers.Count - 1; index > -1; index--) {
                var modelClassMember = modelBoModelClassMembers[index];
                var xpandCustomMemberInfo = xpClassInfo.FindMember(modelClassMember.Name) as XpandCustomMemberInfo;
                if (xpandCustomMemberInfo != null && xpandCustomMemberInfo.HasAttribute(typeof(ModelMemberExMemberInfoAttribute))) {
                    modelClassMember.Remove();
                }
            }
        }
    }

    public class BOModelNodesUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {


        public override void UpdateNode(ModelNode node) {
            var boModel = ((IModelBOModel)node);
            var classNode = boModel[typeof(RoleModelDifferenceObject).FullName];
            if (SecuritySystem.UserType != null && !(SecuritySystem.Instance is ISecurityComplex) && classNode != null) {
                (classNode).Remove();
            }
        }
    }
}
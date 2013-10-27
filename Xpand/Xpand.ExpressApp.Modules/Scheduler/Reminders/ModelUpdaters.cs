using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public class ReminderInfoAppearenceRuleUpdater:ModelNodesGeneratorUpdater<AppearanceRulesModelNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelClass = ((IModelClass) node.Parent);
            if (!AddRule(node, modelClass)) {
                var modelApplication = ((IModelApplicationResourceDifferences) node.Application).ResourceDifferencesApplication;
                if (modelApplication.BOModel != null) {
                    modelClass = modelApplication.BOModel.GetClass(modelClass.TypeInfo.Type);
                    if (modelClass != null) {
                        AddRule(node, modelClass);
                    }
                }
            }
        }

        bool AddRule(ModelNode node, IModelClass modelClass) {
            if (modelClass.OwnMembers != null) {
                var modelMemberReminderInfo = modelClass.OwnMembers.OfType<IModelMemberReminderInfo>().FirstOrDefault();
                if (modelMemberReminderInfo != null) {
                    var modelAppearanceRule =((IModelAppearanceRules) node).AddNode<IModelAppearanceRule>("ReminderInfo.TimeBeforeStart");
                    modelAppearanceRule.AppearanceItemType = AppearanceItemType.ViewItem.ToString();
                    modelAppearanceRule.Criteria = "!" + modelMemberReminderInfo.Name + ".HasReminder";
                    modelAppearanceRule.Enabled = false;
                    modelAppearanceRule.TargetItems = modelMemberReminderInfo.Name;
                    return true;
                }
            }
            return false;
        }
    }

    public class ReminderInfoModelMemberUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelBoModelClassMembers = ((IModelBOModelClassMembers) node);
            var modelClass = ((IModelClass) modelBoModelClassMembers.Parent);
            var supportsReminderAttribute = modelClass.TypeInfo.FindAttribute<SupportsReminderAttribute>();
            if (supportsReminderAttribute!=null) {
                var modelMemberReminderInfo = modelBoModelClassMembers.AddNode<IModelMemberReminderInfo>(supportsReminderAttribute.MemberName);
                modelMemberReminderInfo.Name = supportsReminderAttribute.MemberName;
                modelMemberReminderInfo.ReminderCriteria=supportsReminderAttribute.Criteria;
                var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(modelMemberReminderInfo.MemberInfo.Owner.Type);
                XPMemberInfo xpMemberInfo = classInfo.FindMember(modelMemberReminderInfo.Name);
                ModelMemberReminderInfoDomainLogic.ModifyModel(modelMemberReminderInfo, xpMemberInfo);
            }
        }
    }
}

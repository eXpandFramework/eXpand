using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public class ReminderInfoAppearenceRuleUpdater:ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelApplicationBases = GetModulesDifferences(node);
            var modelClasses = modelApplicationBases.Cast<IModelApplication>().SelectMany(modelApplication 
                => modelApplication.BOModel.Where(HasReminderMember));
            foreach (var modelClass in modelClasses) {
                AddRule(node, modelClass);
            }
        }

        IEnumerable<ModelApplicationBase> GetModulesDifferences(ModelNode node) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            foreach (var moduleBase in ((IModelSources)node.Application).Modules) {
                var modelApplicationBase = node.CreatorInstance.CreateModelApplication();
                modelApplicationBase.Id = moduleBase.Name;
                var resourcesModelStore = new ResourcesModelStore(moduleBase.GetType().Assembly);
                resourcesModelStore.Load(modelApplicationBase);
                var modelBoModel = ((IModelApplication) modelApplicationBase).BOModel;
                if (modelBoModel != null&&modelBoModel.Any(HasReminderMember))
                    modelApplicationBases.Add(modelApplicationBase);
            }
            return modelApplicationBases;
        }

        bool HasReminderMember(IModelClass @class) {
            return @class.OwnMembers != null && @class.OwnMembers.OfType<IModelMemberReminderInfo>().Any();
        }

        void AddRule(ModelNode node, IModelClass modelClass) {
            if (modelClass.OwnMembers != null) {
                var modelMemberReminderInfo = modelClass.OwnMembers.OfType<IModelMemberReminderInfo>().FirstOrDefault();
                if (modelMemberReminderInfo != null) {
                    var conditionalAppearance = (IModelConditionalAppearance)node.Application.BOModel.GetClass(modelClass.TypeInfo.Type);
                    var modelAppearanceRule = conditionalAppearance.AppearanceRules.AddNode<IModelAppearanceRule>("ReminderInfo.TimeBeforeStart");
                    modelAppearanceRule.AppearanceItemType = AppearanceItemType.ViewItem.ToString();
                    modelAppearanceRule.Criteria = "!" + modelMemberReminderInfo.Name + ".HasReminder";
                    modelAppearanceRule.Enabled = false;
                    modelAppearanceRule.TargetItems = modelMemberReminderInfo.Name;
                }
            }
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

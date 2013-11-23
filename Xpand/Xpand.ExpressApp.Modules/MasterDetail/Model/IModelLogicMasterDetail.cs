using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;
using System.Linq;

namespace Xpand.ExpressApp.MasterDetail.Model {
    public interface IModelLogicMasterDetail:IModelNode  {
        IModelMasterDetailLogicRules Rules { get; }
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
        IModelActionsExcludedFromSynchronization ActionsExcludedFromSynchronization { get; }

    }

    [ModelNodesGenerator(typeof(ExcludedActionsFromSynchronizationNodeGenerator))]
    public interface IModelActionsExcludedFromSynchronization : IModelNode, IModelList<IModelActionExcludedFromSynchronization> {
    }

    public class ExcludedActionsFromSynchronizationNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            var actionContainers = new[] { "ViewsHistoryNavigation", "ViewsNavigation", "Tools", "Security", "PopupActions" };
            var actionToContainerMapping = ((IModelActionDesignContainerMapping)node.Application.ActionDesign).ActionToContainerMapping;
            foreach (var actionContainer in actionContainers.Select(s => actionToContainerMapping[s])) {
                foreach (var link in actionContainer) {
                    var actionContext = ((IModelActionsExcludedFromSynchronization)node).AddNode<IModelActionExcludedFromSynchronization>(link.ActionId);
                    actionContext.Action = link.Action;
                }
            }
        }
    }

    [KeyProperty("ActionId")]
    public interface IModelActionExcludedFromSynchronization:IModelNode {
        [Browsable(false)]
        string ActionId { get; set; }
        [DataSourceProperty("Application.ActionDesign.Actions")]
        [Required]
        IModelAction Action { get; set; }
    }

    [DomainLogic(typeof(IModelActionExcludedFromSynchronization))]
    public static class IModelActionExcludedFromSynchronizationLogic {
        public static IModelAction Get_Action(IModelActionExcludedFromSynchronization modelAction) {
            return modelAction.Application.ActionDesign.Actions[modelAction.ActionId];
        }
        public static void Set_Action(IModelActionExcludedFromSynchronization modelAction, IModelAction value) {
            modelAction.ActionId = value.Id;
        }
    }

    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelMasterDetailLogicRules:IModelNode,IModelList<IModelMasterDetailRule> {
    }
}
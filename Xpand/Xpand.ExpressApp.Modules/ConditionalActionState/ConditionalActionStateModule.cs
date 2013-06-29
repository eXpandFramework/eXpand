using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.ExpressApp.ConditionalActionState.Model;
using Xpand.ExpressApp.ConditionalActionState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalActionState {

    [ToolboxItem(false)]
    public sealed class ConditionalActionStateModule : LogicModuleBase<IActionStateRule, ActionStateRule, IModelActionStateRule, IModelApplicationModelArtifactState, IModelLogicConditionalActionState> {

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ExecutionContext.ViewChanging}; }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
         
        }

        public override LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule, IModelApplicationModelArtifactState> LogicRulesNodeUpdater {
            get { return new ActionStateRulesNodeUpdater(); }
        }

        public override IModelLogicConditionalActionState GetModelLogic(IModelApplicationModelArtifactState applicationModel) {
            return applicationModel.ModelArtifactState.ConditionalActionState;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(IModelActionStateRule)).AddAttribute(new ModelEditorLogicRuleAttribute(typeof(IModelLogicConditionalActionState)));
        }

        #region IModelExtender Members


        #endregion
    }
}
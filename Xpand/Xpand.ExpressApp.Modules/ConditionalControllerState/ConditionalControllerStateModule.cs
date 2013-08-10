using System.Collections.Generic;
using System.ComponentModel;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Model;
using Xpand.ExpressApp.ConditionalControllerState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalControllerState {

    [ToolboxItem(false)]
    public sealed class ConditionalControllerStateModule : LogicModuleBase<IControllerStateRule, ControllerStateRule, IModelControllerStateRule, IModelApplicationModelArtifactState, IModelLogicConditionalControllerState> {
        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ ExecutionContext.ViewChanging }; }
        }

        public override LogicRulesNodeUpdater<IControllerStateRule, IModelControllerStateRule, IModelApplicationModelArtifactState> LogicRulesNodeUpdater {
            get { return new ControllerStateRulesNodeUpdater(); }
        }

        public override IModelLogicConditionalControllerState GetModelLogic(IModelApplicationModelArtifactState applicationModel) {
            return applicationModel.ModelArtifactState.ConditionalControllerState;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(IModelControllerStateRule)).AddAttribute(new ModelEditorLogicRuleAttribute(typeof(IModelLogicConditionalControllerState)));
        }

    }
}
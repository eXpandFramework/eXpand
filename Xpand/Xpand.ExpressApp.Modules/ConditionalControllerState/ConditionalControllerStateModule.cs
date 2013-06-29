using System.Collections.Generic;
using System.ComponentModel;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Model;
using Xpand.ExpressApp.ConditionalControllerState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalControllerState {

    [ToolboxItem(false)]
    public sealed class ConditionalControllerStateModule : LogicModuleBase<IControllerStateRule, ControllerStateRule, IModelControllerStateRule, IModelArtifactState,IModelLogicConditionalControllerState> {
        public ConditionalControllerStateModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ ExecutionContext.ViewChanging }; }
        }

        public override LogicRulesNodeUpdater<IControllerStateRule, IModelControllerStateRule, IModelArtifactState> LogicRulesNodeUpdater {
            get { return new ControllerStateRulesNodeUpdater(); }
        }

        public override IModelLogicConditionalControllerState GetModelLogic(IModelArtifactState modelArtifactState) {
            return modelArtifactState.ConditionalControllerState;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.FindTypeInfo(typeof(IModelControllerStateRule)).AddAttribute(new ModelEditorLogicRuleAttribute(typeof(IModelLogicConditionalControllerState)));
        }

    }
}
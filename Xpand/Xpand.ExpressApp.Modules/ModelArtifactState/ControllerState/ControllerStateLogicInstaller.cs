using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState {
    public class ControllerStateLogicInstaller : LogicInstaller<IControllerStateRule, IModelControllerStateRule> {
        public ControllerStateLogicInstaller(XpandModuleBase xpandModuleBase)
            : base(xpandModuleBase) {

        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext> { ExecutionContext.ViewChanging }; }
        }

        public override LogicRulesNodeUpdater<IControllerStateRule, IModelControllerStateRule> LogicRulesNodeUpdater {
            get { return new ControllerStateRulesNodeUpdater(); }
        }

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            var controllerState = ((IModelApplicationModelArtifactState) applicationModel).ModelArtifactState.ConditionalControllerState;
            return new ModelLogicWrapper(controllerState.Rules, controllerState);
        }
    }
}
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Model;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateLogicInstaller : LogicInstaller<IActionStateRule, IModelActionStateRule> {
        public ActionStateLogicInstaller(XpandModuleBase xpandModuleBase)
            : base(xpandModuleBase) {

        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext> { ExecutionContext.ViewChanging }; }
        }

        public override LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule> LogicRulesNodeUpdater {
            get { return new ActionStateRulesNodeUpdater(); }
        }

        protected override IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel) {
            var conditionalActionState = ((IModelApplicationModelArtifactState) applicationModel).ModelArtifactState.ConditionalActionState;
            return new ModelLogicWrapper(conditionalActionState.Rules, conditionalActionState);
        }

    }
}
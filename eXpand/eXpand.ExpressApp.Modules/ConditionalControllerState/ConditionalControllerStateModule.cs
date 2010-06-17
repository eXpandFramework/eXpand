using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalControllerState.Logic;
using eXpand.ExpressApp.ConditionalControllerState.NodeUpdaters;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Validation;

namespace eXpand.ExpressApp.ConditionalControllerState {
    public sealed class ConditionalControllerStateModule : LogicModuleBase<IControllerStateRule, ControllerStateRule>{
        public ConditionalControllerStateModule() {
            RequiredModuleTypes.Add(typeof (eXpandValidationModule));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ControllerStateDefaultGroupContextNodeUpdater());
            updaters.Add(new ControllerStateDefaultContextNodeUpdater());
            updaters.Add(new ControllerStateRulesNodeUpdater());
        }

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationModelArtifactState)applicationModel).ModelArtifactState.ConditionalControllerState;
        }

        
    }
}
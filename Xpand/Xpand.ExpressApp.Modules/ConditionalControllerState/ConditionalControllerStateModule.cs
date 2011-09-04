using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Validation;

namespace Xpand.ExpressApp.ConditionalControllerState {
    
    [ToolboxItem(false)]
    public sealed class ConditionalControllerStateModule : LogicModuleBase<IControllerStateRule, ControllerStateRule>{
        public ConditionalControllerStateModule() {
            RequiredModuleTypes.Add(typeof (XpandValidationModule));
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
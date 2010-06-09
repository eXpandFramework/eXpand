using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.ConditionalActionState.NodeUpdaters;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ConditionalActionState {
    public sealed class ConditionalActionStateModule : LogicModuleBase<IActionStateRule, ActionStateRule>
    {
        
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ActionStateDefaultGroupContextNodeUpdater());
            updaters.Add(new ActionStateDefaultContextNodeUpdater());
            updaters.Add(new ActionStateRulesNodeUpdater());
        }

        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationModelArtifactState)applicationModel).ModelArtifactState.ConditionalActionState;
        }

        #region IModelExtender Members


        #endregion
    }
}
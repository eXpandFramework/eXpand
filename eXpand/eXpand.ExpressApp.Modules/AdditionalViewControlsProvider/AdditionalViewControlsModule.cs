using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    public sealed class AdditionalViewControlsModule : LogicModuleBase<IAdditionalViewControlsRule,AdditionalViewControlsRule>,IModelExtender
    {
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelApplicationAdditionalViewControls>();
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AdditionalViewControlsDefaultGroupContextNodeUpdater());
            updaters.Add(new AdditionalViewControlsRulesNodeUpdater());
        }
        #endregion
        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationAdditionalViewControls)applicationModel).AdditionalViewControls;
        }
    }
}

using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.NodeUpdaters;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsProviderAspNetModule : ModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }
        public override void AddGeneratorUpdaters(DevExpress.ExpressApp.Model.Core.ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AdditionalViewControlsDefaultContextNodeUpdater());
        }

    }
}
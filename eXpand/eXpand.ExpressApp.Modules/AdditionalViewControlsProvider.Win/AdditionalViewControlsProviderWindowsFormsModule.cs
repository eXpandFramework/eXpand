using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.NodeUpdaters;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win {
    public class AdditionalViewControlsProviderWindowsFormsModule : ModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }
        public override void AddGeneratorUpdaters(DevExpress.ExpressApp.Model.Core.ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AdditionalViewControlsDefaultContextNodeUpdater());
        }
    }
}
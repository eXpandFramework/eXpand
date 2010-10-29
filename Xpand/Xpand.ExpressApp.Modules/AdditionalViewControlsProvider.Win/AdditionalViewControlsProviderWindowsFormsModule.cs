using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win {
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
            RequiredModuleTypes.Add(typeof (XpandSystemWindowsFormsModule));
        }
    }
}
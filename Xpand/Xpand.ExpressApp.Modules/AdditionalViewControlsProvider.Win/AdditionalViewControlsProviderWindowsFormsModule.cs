using Xpand.ExpressApp;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win {
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }
    }
}
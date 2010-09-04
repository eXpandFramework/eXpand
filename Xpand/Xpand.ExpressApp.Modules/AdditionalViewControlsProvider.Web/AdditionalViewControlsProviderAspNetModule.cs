using Xpand.ExpressApp;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsProviderAspNetModule : XpandModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }

    }
}
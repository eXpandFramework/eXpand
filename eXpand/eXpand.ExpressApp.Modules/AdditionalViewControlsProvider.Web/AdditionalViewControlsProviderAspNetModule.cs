namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsProviderAspNetModule : XpandModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }

    }
}
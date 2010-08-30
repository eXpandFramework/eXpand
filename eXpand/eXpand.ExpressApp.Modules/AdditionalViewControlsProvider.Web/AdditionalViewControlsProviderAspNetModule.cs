namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public class AdditionalViewControlsProviderAspNetModule : ModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }

    }
}
namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win {
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }
    }
}
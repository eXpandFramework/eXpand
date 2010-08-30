namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win {
    public class AdditionalViewControlsProviderWindowsFormsModule : ModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (AdditionalViewControlsModule));
        }
    }
}
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace ImportWizardTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.OptimizedControllersCreation = true;
            application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
        }
    }
}

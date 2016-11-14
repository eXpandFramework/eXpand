using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace PivotChartTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
        }
    }
}

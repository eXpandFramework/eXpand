using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace HtmlPropertyEditorTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
        }
    }
}

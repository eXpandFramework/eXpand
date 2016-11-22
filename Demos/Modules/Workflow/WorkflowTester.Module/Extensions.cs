using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;

namespace WorkflowTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            if (!application.GetEasyTestParameter("SqlServer"))
                application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
        }
    }
}

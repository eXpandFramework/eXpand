using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.Security;

namespace SecurityTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            if (!application.GetEasyTestParameter("SqlServer"))
                application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            application.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>();
        }
    }
}

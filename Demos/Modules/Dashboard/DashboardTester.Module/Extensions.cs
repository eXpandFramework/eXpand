using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.Security;

namespace DashboardTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            application.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>(typeof(AuthenticationStandard), typeof(AuthenticationStandardLogonParameters));
        }
    }
}

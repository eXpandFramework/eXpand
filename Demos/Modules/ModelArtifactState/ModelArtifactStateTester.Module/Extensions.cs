using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.BaseImpl.Security;

namespace ModelArtifactStateTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            if (!InterfaceBuilder.IsDevMachine)
                application.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            application.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>(typeof(AuthenticationStandard), typeof(AuthenticationStandardLogonParameters));
        }
    }
}

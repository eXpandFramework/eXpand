using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.Security;

namespace WorldCreatorTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application) {
            application.OptimizedControllersCreation = true;
            application.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>(typeof(AuthenticationStandard), typeof(AuthenticationStandardLogonParameters));
        }
    }
}

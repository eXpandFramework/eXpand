using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.Security;

namespace SystemTester.Module {
    public static class Extensions {
        public static void ProjectSetup(this XafApplication application){
            if (application.GetEasyTestParameter("MySQL"))
                application.ConnectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            else if (application.GetEasyTestParameter("SqlLite"))
                application.ConnectionString = ConfigurationManager.ConnectionStrings["SqlLite"].ConnectionString;
            application.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>(typeof(AuthenticationStandard),typeof(AuthenticationStandardLogonParameters));
        }
    }
}

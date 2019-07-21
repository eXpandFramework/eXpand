using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.Security;
#if !EASYTEST
using DevExpress.ExpressApp.Win.Utils;
#endif

namespace XVideoRental.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            ModelApplicationCreator.DebugMode = true;
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            var winApplication = new XVideoRentalWindowsFormsApplication();

#if EASYTEST
            
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#else
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            winApplication.SplashScreen = new DXSplashScreen();
#endif
            
            try {
                winApplication.UseOldTemplates=false;
                winApplication.NewSecurityStrategyComplexV2<XpandPermissionPolicyUser, XpandPermissionPolicyRole>(typeof(AuthenticationStandard),typeof(AuthenticationStandardLogonParameters));

                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }

    }
}

using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;

namespace SystemTester.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            var winApplication = new SystemTesterWindowsFormsApplication();
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
            try {
                winApplication.NewSecurityStrategyComplex<AuthenticationStandard, AuthenticationStandardLogonParameters>();
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}

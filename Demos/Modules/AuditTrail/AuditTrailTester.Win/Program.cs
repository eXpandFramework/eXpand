using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace AuditTrailTester.Win {
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
            EditModelPermission.AlwaysGranted = true;
            AuditTrailTesterWindowsFormsApplication winApplication = new AuditTrailTesterWindowsFormsApplication();
            // Refer to the http://documentation.devexpress.com/#Xaf/CustomDocument2680 help article for more details on how to provide a custom splash form.
            //winApplication.SplashScreen = new DevExpress.ExpressApp.Win.Utils.DXSplashScreen("YourSplashImage.png");
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#else
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#endif
            try {
                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}

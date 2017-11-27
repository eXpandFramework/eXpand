using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTester.Module;
using SystemTester.Module.FunctionalTests;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.EasyTest;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Ribbon;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.Persistent.Base.General;

namespace SystemTester.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
            EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = Debugger.IsAttached;
            var winApplication = new SystemTesterWindowsFormsApplication();
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
            try{
                winApplication.ProjectSetup();
                winApplication.UseOldTemplates=false;
                winApplication.Setup();
                if (winApplication.GetEasyTestParameter(EasyTestParameter.Ribbon)) {
                    var modelOptionsWin = ((IModelOptionsWin)winApplication.Model.Options);
                    modelOptionsWin.FormStyle = RibbonFormStyle.Ribbon;
                    modelOptionsWin.UIType = UIType.TabbedMDI;
                }
                winApplication.Start();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}

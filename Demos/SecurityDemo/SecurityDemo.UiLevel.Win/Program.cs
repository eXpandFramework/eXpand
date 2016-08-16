using System;
using System.Configuration;
using System.Windows.Forms;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Demos;

using SecurityDemo.Module;
using DevExpress.Internal;

namespace SecurityDemo.UiLevel.Win {
    static class Program {
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arguments) {
#if EasyTest
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SecurityDemoWindowsFormsApplication application = new SecurityDemoWindowsFormsApplication();
            application.CreateCustomTemplate += new EventHandler<CreateCustomTemplateEventArgs>(xafApplication_CreateCustomTemplate);
            application.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(application_CreateCustomLogonWindowObjectSpace);
            application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(application_CreateCustomLogonWindowControllers);
#if EasyTest
                if (ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				application.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#else
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                application.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#endif

            if (System.Diagnostics.Debugger.IsAttached && application.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }

            try {
                application.Setup();
                application.Start();
            }
            catch(Exception e) {
                application.HandleException(e);
            }
        }

        static void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(((XafApplication)sender).CreateController<ShowHintController>());
        }

        static void application_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = ((XafApplication)sender).CreateObjectSpace();
            ((SecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        static void xafApplication_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e) {
            if(e.Context.Name == TemplateContext.ApplicationWindow) {
                e.Template = new FeatureCenter.Module.Win.MainForm();
            }
            if(e.Context.Name == TemplateContext.PopupWindow) {
                e.Template = new FeatureCenter.Module.Win.PopupForm();
            }
        }
    }
}

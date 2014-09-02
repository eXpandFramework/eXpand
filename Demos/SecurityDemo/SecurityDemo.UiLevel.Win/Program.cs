using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using FeatureCenter.Module.Win;
using SecurityDemo.Module;

namespace SecurityDemo.UiLevel.Win{
    internal static class Program{
        /// The main entry point for the application.
        /// 
        [STAThread]
        private static void Main(){

#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var application = new SecurityDemoWindowsFormsApplication();
            application.CreateCustomTemplate += xafApplication_CreateCustomTemplate;
            application.CreateCustomLogonWindowObjectSpace += application_CreateCustomLogonWindowObjectSpace;
            application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				application.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#else
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null){
                application.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#endif
            try{
                application.Setup();
                application.Start();
            }
            catch (Exception e){
                application.HandleException(e);
            }
        }

        private static void application_CreateCustomLogonWindowControllers(object sender,
            CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(((XafApplication) sender).CreateController<ShowHintController>());
        }

        private static void application_CreateCustomLogonWindowObjectSpace(object sender,
            CreateCustomLogonWindowObjectSpaceEventArgs e){
            e.ObjectSpace = ((XafApplication) sender).CreateObjectSpace();
            ((SecurityDemoAuthenticationLogonParameters) e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        private static void xafApplication_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e){
            if (e.Context.Name == TemplateContext.ApplicationWindow){
                e.Template = new MainForm();
            }
            if (e.Context.Name == TemplateContext.PopupWindow){
                e.Template = new PopupForm();
            }
        }
    }
}
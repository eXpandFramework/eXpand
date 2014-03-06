using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using FeatureCenter.Module.Win;
using SecurityDemo.Module;

namespace SecurityDemo.Win{
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
                var security = new SecurityStrategyComplex(typeof (SecuritySystemUser), typeof (SecuritySystemRole),
                    new SecurityDemoAuthentication());
                application.Security = security;

                //                This functionality is build in the XpandObjectSpaceProvider. To enable or disable use Model/Options/ClientSideSecurity. You can continue using the designer if you want.

                //                application.CreateCustomObjectSpaceProvider += delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                //                    e.ObjectSpaceProvider = new SecuredObjectSpaceProvider(security, e.ConnectionString, e.Connection);
                //                };
                application.DatabaseVersionMismatch += delegate(object sender, DatabaseVersionMismatchEventArgs e){
                    try{
#if EASYTEST
                        e.Updater.Update();
                        e.Handled = true;
#else
                        if (Debugger.IsAttached){
                            e.Updater.Update();
                            e.Handled = true;
                        }
#endif
                    }
                    catch (CompatibilityException exception){
                        if (exception.Error is CompatibilityUnableToOpenDatabaseError){
                            throw new UserFriendlyException(
                                "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                        }
                    }
                };

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
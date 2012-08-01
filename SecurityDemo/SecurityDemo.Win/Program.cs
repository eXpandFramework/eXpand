using System;
using System.Configuration;
using System.Windows.Forms;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Demos;

using SecurityDemo.Module;
using DevExpress.ExpressApp.MiddleTier.Remoting;
using DevExpress.ExpressApp.MiddleTier;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Xpo;
using DevExpress.Utils;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;
using System.Runtime.Remoting;
using System.ServiceModel;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using DevExpress.ExpressApp.Security;

namespace SecurityDemo.Win {
    static class Program {
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arguments) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SecurityDemoWindowsFormsApplication application = new SecurityDemoWindowsFormsApplication();
            application.CreateCustomTemplate += new EventHandler<CreateCustomTemplateEventArgs>(xafApplication_CreateCustomTemplate);
            application.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(application_CreateCustomLogonWindowObjectSpace);
            application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(application_CreateCustomLogonWindowControllers);

            try {
                application.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(SecurityDemoUser), typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole), new SecurityDemoAuthentication());
                application.Security = security;
                
                //You do not need this call with Xpand the functionality is build in


                //                application.CreateCustomObjectSpaceProvider += delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                //                    e.ObjectSpaceProvider = new SecuredObjectSpaceProvider(security, e.ConnectionString, e.Connection);
                //                };
                application.DatabaseVersionMismatch += delegate(object sender, DatabaseVersionMismatchEventArgs e) {
                    e.Updater.Update();
                    e.Handled = true;
                };

                application.Setup();
                application.Start();
            } catch (Exception e) {
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
            if (e.Context.Name == TemplateContext.ApplicationWindow) {
                e.Template = new FeatureCenter.Module.Win.MainForm();
            }
            if (e.Context.Name == TemplateContext.PopupWindow) {
                e.Template = new FeatureCenter.Module.Win.PopupForm();
            }
        }
    }
}

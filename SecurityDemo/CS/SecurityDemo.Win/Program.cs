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
using DevExpress.ExpressApp.MiddleTier.Remoting;
using DevExpress.ExpressApp.MiddleTier;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Xpand.ExpressApp;

namespace SecurityDemo.Win {
	static class Program {
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] arguments) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            KnownTypesProvider.KnownTypes.Add(typeof(SecurityDemoAuthenticationLogonParameters));
			SecurityDemoWindowsFormsApplication application = new SecurityDemoWindowsFormsApplication();
            application.CreateCustomTemplate += new EventHandler<CreateCustomTemplateEventArgs>(xafApplication_CreateCustomTemplate);
            application.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(application_CreateCustomLogonWindowObjectSpace);
            application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(application_CreateCustomLogonWindowControllers);

			if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
				application.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
			}

			try {
                ApplicationServerStarter starter = new ApplicationServerStarter();
                starter.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                starter.ServerConnectionString = ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString;
                starter.Start();

                new MiddleTierClientApplicationConfigurator(application);
			    ((ISupportFullConnectionString) application).ConnectionString =
			        ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
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

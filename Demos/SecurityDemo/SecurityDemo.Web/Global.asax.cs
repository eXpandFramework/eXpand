using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using SecurityDemo.Module;
using DevExpress.ExpressApp.Web.Demos;
using DevExpress.Web.ASPxClasses;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security.Strategy;

namespace SecurityDemo.Web {
	public class Global : System.Web.HttpApplication {
        //private static ApplicationServerStarter starter = null;
		public Global() {
            InitializeComponent();
		}
		protected void Application_Start(Object sender, EventArgs e) {
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
		}
		protected void Session_Start(Object sender, EventArgs e) {
			WebApplication.SetInstance(Session, new SecurityDemoAspNetApplication());
            WebApplication.Instance.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(Instance_CreateCustomLogonWindowObjectSpace);
            WebApplication.Instance.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Instance_CreateCustomLogonWindowControllers);


            if(ConfigurationManager.AppSettings["SiteMode"] != null && ConfigurationManager.AppSettings["SiteMode"].ToLower() == "true") {
                InMemoryDataStoreProvider.Register();
                WebApplication.Instance.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            }
            else {
                if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                    WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
            }
            SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(SecuritySystemUser), typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole), new SecurityDemoAuthentication());
            WebApplication.Instance.Security = security;
            WebApplication.Instance.DatabaseVersionMismatch += delegate(object sender2, DatabaseVersionMismatchEventArgs e2) {
                try {
                    e2.Updater.Update();
                    e2.Handled = true;
                }
                catch(CompatibilityException exception) {
                    if(exception.Error is CompatibilityUnableToOpenDatabaseError) {
                        throw new UserFriendlyException(
                        "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                    }
                }
            };

//            WebApplication.Instance.CreateCustomObjectSpaceProvider += delegate(object sender3, CreateCustomObjectSpaceProviderEventArgs e3) {
//                e3.ObjectSpaceProvider = new SecuredObjectSpaceProvider(security, e3.ConnectionString, e3.Connection);
//            };
            
            WebApplication.Instance.Setup();
			WebApplication.Instance.Start();

        }

        private void Instance_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(((XafApplication)sender).CreateController<DevExpress.ExpressApp.Demos.ShowHintController>());
        }

        private void Instance_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = ((XafApplication)sender).CreateObjectSpace();
            ((SecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

		protected void Application_BeginRequest(Object sender, EventArgs e) {
			string filePath = HttpContext.Current.Request.PhysicalPath;
			if(!string.IsNullOrEmpty(filePath)
				&& (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
				HttpContext.Current.Response.End();
			}
		}
		protected void Application_EndRequest(Object sender, EventArgs e) {
		}
		protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
		}
		protected void Application_Error(Object sender, EventArgs e) {
			ErrorHandling.Instance.ProcessApplicationError();
		}
		protected void Session_End(Object sender, EventArgs e) {
			WebApplication.DisposeInstance(Session);
		}
		protected void Application_End(Object sender, EventArgs e) {
		}
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
		}
		#endregion
	}
}

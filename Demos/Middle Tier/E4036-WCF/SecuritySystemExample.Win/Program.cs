using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using Xpand.Persistent.Base.MiddleTier;

namespace SecuritySystemExample.Win {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = Debugger.IsAttached;
            var winApplication =new SecuritySystemExampleWindowsFormsApplication();
            try {
                winApplication.ConnectionString = "http://localhost:1451/DataServer";
                XpandWcfDataServerHelper.AddKnownTypes();
                var defaultBinding = (WSHttpBinding) WcfDataServerHelper.CreateDefaultBinding();
                defaultBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
                var clientDataServer = new WcfSecuredDataServerClient(
                    defaultBinding,
                    new EndpointAddress(winApplication.ConnectionString));
                var securityClient =new ServerSecurityClient(clientDataServer, new ClientInfoFactory()){IsSupportChangePassword = true};
                winApplication.ApplicationName = "SecuritySystemExample";
                winApplication.Security = securityClient;
                winApplication.CreateCustomObjectSpaceProvider +=
                    delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                        e.ObjectSpaceProvider =new DataServerObjectSpaceProvider(clientDataServer, securityClient);
                    };
                winApplication.Setup();
                winApplication.Start();
                clientDataServer.Close();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
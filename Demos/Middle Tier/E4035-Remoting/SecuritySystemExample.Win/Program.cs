using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;

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
            var winApplication =
                new SecuritySystemExampleWindowsFormsApplication();
            const string connectionString = "tcp://localhost:1426/DataServer";
            winApplication.ConnectionString = connectionString;
            try {
                var t = new Hashtable{{"secure", true}, {"tokenImpersonationLevel", "impersonation"}};
                var channel = new TcpChannel(t, null, null);
                ChannelServices.RegisterChannel(channel, true);
                var clientDataServer = (IDataServer) Activator.GetObject(
                    typeof (RemoteSecuredDataServer), connectionString);
                var securityClient =
                    new ServerSecurityClient(clientDataServer, new ClientInfoFactory()){IsSupportChangePassword = true};
                winApplication.ApplicationName = "SecuritySystemExample";
                winApplication.Security = securityClient;
                winApplication.CreateCustomObjectSpaceProvider +=
                    delegate(object sender, CreateCustomObjectSpaceProviderEventArgs e) {
                        e.ObjectSpaceProvider =
                            new DataServerObjectSpaceProvider(clientDataServer, securityClient);
                    };
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
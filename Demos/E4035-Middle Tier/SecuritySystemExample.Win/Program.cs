// Developer Express Code Central Example:
// How to: Implement middle tier security with the .NET Remoting service
// 
// The complete description is available in the Middle Tier Security - .NET
// Remoting Service (http://documentation.devexpress.com/#xaf/CustomDocument3438)
// topic.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4035

using System;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using System.Collections;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;
using Xpand.Persistent.Base.General;

namespace SecuritySystemExample.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            XafApplicationExtensions.DisableObjectSpaceProderCreation = true;
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            SecuritySystemExampleWindowsFormsApplication winApplication = 
                new SecuritySystemExampleWindowsFormsApplication();
            string connectionString = "tcp://localhost:1425/DataServer";
            winApplication.ConnectionString = connectionString;
            try {
                Hashtable t = new Hashtable();
                t.Add("secure", true);
                t.Add("tokenImpersonationLevel", "impersonation");
                TcpChannel channel = new TcpChannel(t, null, null);
                ChannelServices.RegisterChannel(channel, true);
                IDataServer clientDataServer = (IDataServer)Activator.GetObject(
                    typeof(RemoteSecuredDataServer), connectionString);
                ServerSecurityClient securityClient =
                    new ServerSecurityClient(clientDataServer, new ClientInfoFactory());
                securityClient.IsSupportChangePassword = true;
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

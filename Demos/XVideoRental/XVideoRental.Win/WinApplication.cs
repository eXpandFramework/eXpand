using System;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Win;

namespace XVideoRental.Win {
    public partial class XVideoRentalWindowsFormsApplication : XpandWinApplication {
        public XVideoRentalWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersRead += OnLastLogonParametersRead;
            SplashScreen = null;
        }

        void OnLastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            var logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if (logonParameters != null) {
                if (String.IsNullOrEmpty(logonParameters.UserName)) {
                    logonParameters.UserName = "Admin";
                }
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        void XVideoRentalWindowsFormsApplication_DatabaseVersionMismatch(object sender,
                                                                         DatabaseVersionMismatchEventArgs e) {
            if (this.DropDatabaseOnVersionMissmatch() > 0)
                Application.ExitThread();
#if DEBUG
            e.Updater.Update();
            e.Handled = true;
#else
            if (true) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " +
                    "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
            }
#endif
        }

        void XVideoRentalWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}
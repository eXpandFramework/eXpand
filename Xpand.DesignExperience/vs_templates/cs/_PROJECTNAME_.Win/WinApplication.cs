using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win;
using DevExpress.ExpressApp.Security;

namespace $projectsuffix$.Win {
    public partial class $projectsuffix$WindowsFormsApplication : XpandWinApplication {
        public $projectsuffix$WindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersRead += OnLastLogonParametersRead;
        }

        void OnLastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            AuthenticationStandardLogonParameters logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if (logonParameters != null) {
                if (String.IsNullOrEmpty(logonParameters.UserName)) {
                    logonParameters.UserName = "Admin";
                }
            }
        }
        private void $projectsuffix$WindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            } else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "The automatic update is disabled, because the application was started without debugging.\r\n" +
                    "You should start the application under Visual Studio, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.");
            }
#endif
        }
    }
}

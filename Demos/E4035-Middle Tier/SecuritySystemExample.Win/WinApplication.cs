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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Win;

namespace SecuritySystemExample.Win {
    public partial class SecuritySystemExampleWindowsFormsApplication : XpandWinApplication {
        public SecuritySystemExampleWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new SecuredObjectSpaceProvider(
                (SecurityStrategy)Security, args.ConnectionString, args.Connection);
        }
        private void SecuritySystemExampleWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
            throw new InvalidOperationException(
                "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.");
        }
        private void SecuritySystemExampleWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}

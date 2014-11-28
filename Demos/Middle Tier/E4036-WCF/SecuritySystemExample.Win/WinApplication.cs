using System;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using Xpand.ExpressApp.Win;

namespace SecuritySystemExample.Win {
    public partial class SecuritySystemExampleWindowsFormsApplication : XpandWinApplication {
        public SecuritySystemExampleWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
        }

        protected override string GetModelAssemblyFilePath(){
            return null;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new SecuredObjectSpaceProvider(
                (SecurityStrategy) Security, args.ConnectionString, args.Connection);
        }

        void SecuritySystemExampleWindowsFormsApplication_DatabaseVersionMismatch(object sender,
                                                                                  DatabaseVersionMismatchEventArgs e) {
            throw new InvalidOperationException(
                "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.");
        }

        void SecuritySystemExampleWindowsFormsApplication_CustomizeLanguagesList(object sender,
                                                                                 CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Configuration;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp;
using Xpand.ExpressApp.MiddleTier;

namespace SecurityDemo.Module {
    public abstract class ApplicationServerStarterBase : MarshalByRefObject {
        private AppDomain domain;
        private static ApplicationServerStarterBase starter;
        private string connectionString;
        private string serverConnectionString;

        protected virtual IList<ModuleBase> GetModules() {
            List<ModuleBase> modules = new List<ModuleBase>();
            modules.Add(new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule());
            modules.Add(new DevExpress.ExpressApp.SystemModule.SystemModule());
            return modules;
        }
        private void StartServer() {
            XpandServerApplication serverApplication = new XpandServerApplication();
            serverApplication.ApplicationName = "SecurityDemo";
            serverApplication.DatabaseVersionMismatch += new EventHandler<DatabaseVersionMismatchEventArgs>(serverApplication_DatabaseVersionMismatch);
            serverApplication.SetupComplete += ServerApplicationOnSetupComplete;

            foreach (ModuleBase module in GetModules()) {
                if (!serverApplication.Modules.Contains(module)) {
                    serverApplication.Modules.Add(module);
                }
            }
            serverApplication.ConnectionString = serverConnectionString;

            SecurityDemoAuthentication authentication = new SecurityDemoAuthentication();
            //AuthenticationActiveDirectory authentication = new AuthenticationActiveDirectory(typeof(SecurityDemo.Module.SecurityUser), null);
            //authenticationActiveDirectory.CreateUserAutomatically = true;
            //#region DEMO_REMOVE
            //authentication = new AuthenticationStandardForTests();
            //#endregion

            serverApplication.Security = new SecurityStrategyComplex(typeof(SecurityDemoUser), typeof(SecurityRole), authentication);
            (serverApplication).ConnectionString = serverConnectionString;
            serverApplication.Setup();
            serverApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            serverApplication.CheckCompatibility();

            ApplicationServer applicationServer = new ApplicationServer(connectionString, "SecurityDemoApplicationServer", serverConnectionString);
            applicationServer.ObjectSpaceProvider = serverApplication.ObjectSpaceProvider;
            applicationServer.Security = serverApplication.Security;
            applicationServer.SecurityService = new ServerSecurityStrategyService(authentication);

            try {
                applicationServer.Start();
                SecurityModule.StrictSecurityStrategyBehavior = false;
            } catch (Exception e) {
                Console.WriteLine(e);
            }

        }

        void ServerApplicationOnSetupComplete(object sender, EventArgs eventArgs) {

        }


        void serverApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            try {
                e.Updater.Update();
                e.Handled = true;
            } catch (CompatibilityException exception) {
                if (exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
        protected abstract AppDomain CreateDomain();
        protected abstract ApplicationServerStarterBase CreateServerStarter(AppDomain domain);
        public void Start() {
            domain = CreateDomain();

            starter = CreateServerStarter(domain);
            starter.ConnectionString = connectionString;
            starter.ServerConnectionString = serverConnectionString;

            starter.StartServer();
        }

        public void Stop() {
            if (domain != null) {
                AppDomain.Unload(domain);
            }
        }
        public string ConnectionString {
            get {
                return connectionString;
            }
            set {
                connectionString = value;
            }
        }
        public string ServerConnectionString {
            get {
                return serverConnectionString;
            }
            set {
                serverConnectionString = value;
            }
        }
    }
}

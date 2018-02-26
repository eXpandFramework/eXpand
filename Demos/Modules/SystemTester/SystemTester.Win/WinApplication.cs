#if !EASYTEST
using System;
#endif
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Win {
    public sealed partial class SystemTesterWindowsFormsApplication : WinApplication {
        public SystemTesterWindowsFormsApplication() {
            InitializeComponent();
            LastLogonParametersReading += OnLastLogonParametersReading;
	        IsDelayedDetailViewDataLoadingEnabled = true;
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            if (args.ConnectionString == InMemoryDataStoreProvider.ConnectionString)
                args.ObjectSpaceProvider =
                    new XPObjectSpaceProvider(new ConnectionStringDataStoreProvider(args.ConnectionString));
            else{
                var doNotTrackModifiedObjects = this.GetEasyTestParameter("PessimisticLocking");
                args.ObjectSpaceProvider =
                    new XpandObjectSpaceProvider(new MultiDataStoreProvider(args.ConnectionString), Security,doNotTrackModifiedObjects);
            }

            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider( ));
        }

        class XpandObjectSpaceProvider:Xpand.Persistent.Base.General.XpandObjectSpaceProvider{
            private readonly bool _doNotTrackModifiedObjects;

            public XpandObjectSpaceProvider(IXpoDataStoreProxy provider, ISecurityStrategyBase security, bool doNotTrackModifiedObjects) : base(provider, security){
                _doNotTrackModifiedObjects = doNotTrackModifiedObjects;
            }

            protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer){
                var unitOfWork = base.CreateUnitOfWork(dataLayer);
                unitOfWork.TrackPropertiesModifications = !_doNotTrackModifiedObjects;
                return unitOfWork;
            }
        }
        private void SystemTesterWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }

        private void SystemTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached) {
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
    }
}

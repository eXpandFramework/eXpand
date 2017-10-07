using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;

namespace WorldCreatorTester.Win {
    public partial class WorldCreatorTesterWindowsFormsApplication : WinApplication {
        public WorldCreatorTesterWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersReading += OnLastLogonParametersReading;
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs e){
            var easyTestParameter = this.GetEasyTestParameter("DBMapper");
            e.ObjectSpaceProviders.Add(easyTestParameter
                ? new XpandObjectSpaceProvider(new MultiDataStoreProvider(e.ConnectionString), Security)
                : new XPObjectSpaceProvider(e.ConnectionString));
            e.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        void WorldCreatorTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender,
                                                                               DatabaseVersionMismatchEventArgs e) {
                e.Updater.Update();
                e.Handled = true;
        }

        void WorldCreatorTesterWindowsFormsApplication_CustomizeLanguagesList(object sender,
                                                                              CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}
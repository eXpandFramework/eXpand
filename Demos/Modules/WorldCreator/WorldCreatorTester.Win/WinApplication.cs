using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            this.CreateCustomObjectSpaceprovider(args,null);
        }

#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        void WorldCreatorTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender,
                                                                               DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
                                                                                   if (true) {
                e.Updater.Update();
                e.Handled = true;
            }
#endif
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
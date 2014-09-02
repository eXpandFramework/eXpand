using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using Xpand.Persistent.Base.General;

namespace XpandSystemTester.Win {
    public partial class XpandSystemTesterWindowsFormsApplication : WinApplication {
        public XpandSystemTesterWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersReading+=OnLastLogonParametersReading;
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e){
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args,null);
        }

        void XpandSystemTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender,
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

        void XpandSystemTesterWindowsFormsApplication_CustomizeLanguagesList(object sender,
                                                                             CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}
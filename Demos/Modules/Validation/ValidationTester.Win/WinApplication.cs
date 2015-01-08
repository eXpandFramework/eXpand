using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Security.Core;

namespace ValidationTester.Win {
    public partial class ValidationTesterWindowsFormsApplication : WinApplication {
        public ValidationTesterWindowsFormsApplication() {
            InitializeComponent();
            LastLogonParametersReading += OnLastLogonParametersReading;
            DelayedViewItemsInitialization = true;
            this.NewSecurityStrategyComplex<AuthenticationStandard, AuthenticationStandardLogonParameters>();
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
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        private void ValidationTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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


        private void ValidationTesterWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}

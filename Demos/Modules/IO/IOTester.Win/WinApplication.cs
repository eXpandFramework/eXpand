using System.ComponentModel;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using IOTester.Module;
using Xpand.Persistent.Base.General;

namespace IOTester.Win {
    public partial class IOTesterWindowsFormsApplication : WinApplication, IConfirmationRequired {
        public IOTesterWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersReading += OnLastLogonParametersReading;
        }


        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        public event CancelEventHandler ConfirmationRequired;


        protected void OnConfirmationRequired(CancelEventArgs e) {
            CancelEventHandler handler = ConfirmationRequired;
            if (handler != null) handler(this, e);
        }
        public override ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
            var cancelEventArgs = new CancelEventArgs();
            OnConfirmationRequired(cancelEventArgs);
            return cancelEventArgs.Cancel ? ConfirmationResult.No : base.AskConfirmation(confirmationType);
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = this.GetObjectSpaceProvider(args.ConnectionString);
        }
        private void IOTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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
        private void IOTesterWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
    }
}

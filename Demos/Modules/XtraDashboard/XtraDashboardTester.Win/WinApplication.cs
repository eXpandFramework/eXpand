#if !EASYTEST
using System;
using System.Diagnostics;
#endif
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
namespace XtraDashboardTester.Win{
    public sealed partial class XtraDashboardTesterWindowsFormsApplication : WinApplication{
        public XtraDashboardTesterWindowsFormsApplication(){
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            IsDelayedDetailViewDataLoadingEnabled = true;
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }

        private void XtraDashboardTesterWindowsFormsApplication_CustomizeLanguagesList(object sender,
            CustomizeLanguagesListEventArgs e){
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1){
                e.Languages.Add(userLanguageName);
            }
        }

        private void XtraDashboardTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender,
            DatabaseVersionMismatchEventArgs e){
            e.Updater.Update();
            e.Handled = true;

        }
    }
}
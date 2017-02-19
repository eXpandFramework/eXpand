using System.ComponentModel;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using ModelDifferenceTester.Module;
using ModelDifferenceTester.Module.FunctionalTests;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;

namespace ModelDifferenceTester.Win {
    public partial class ModelDifferenceTesterWindowsFormsApplication : WinApplication {
        public ModelDifferenceTesterWindowsFormsApplication() {
            InitializeComponent();
            DelayedViewItemsInitialization = true;
            LastLogonParametersReading += OnLastLogonParametersReading;
            this.ProjectSetup();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(args.ConnectionString));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider());
            if (this.GetEasyTestParameter(EasyTestParameters.WCModel))
                args.ObjectSpaceProviders.Add(new WorldCreatorObjectSpaceProvider());
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

        void ModelDifferenceTesterWindowsFormsApplication_DatabaseVersionMismatch(object sender,
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

        void ModelDifferenceTesterWindowsFormsApplication_CustomizeLanguagesList(object sender,
                                                                                 CustomizeLanguagesListEventArgs e) {
            string userLanguageName = Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }

    }
}
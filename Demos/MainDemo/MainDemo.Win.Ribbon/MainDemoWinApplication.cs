using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security.ClientServer;

namespace MainDemo.Win {
	public partial class MainDemoWinApplication : WinApplication {
        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
		static MainDemoWinApplication() {
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
        }
        private void InitializeDefaults() {
            LinkNewObjectToParentImmediately = false;
            OptimizedControllersCreation = true;
			EnableModelCache = true;
            UseLightStyle = true;
        }
        #endregion
		public MainDemoWinApplication() {
			InitializeComponent();
			InitializeDefaults();
            
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;
		}
		protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
			args.ObjectSpaceProviders.Add(new SecuredObjectSpaceProvider((ISelectDataSecurityProvider)Security, XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true), false));
			args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
		}
		private void MainDemoWinApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}
        private void MainDemoWinApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            // Just to read demo user name for logon.
            AuthenticationStandardLogonParameters logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if(logonParameters != null) {
                if(String.IsNullOrEmpty(logonParameters.UserName)) {
                    logonParameters.UserName = "Sam";
                }
            }
        }
	}
}

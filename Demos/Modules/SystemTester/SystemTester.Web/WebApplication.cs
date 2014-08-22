using System;
using System.ComponentModel;
using System.Diagnostics;
using SystemTester.Module;
using SystemTester.Module.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;

//using DevExpress.ExpressApp.Security;

namespace SystemTester.Web{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
    public class SystemTesterAspNetApplication : WebApplication{
        private SystemModule _module1;
        private SystemAspNetModule _module2;
        private SystemTesterModule _module3;
        private SystemTesterAspNetModule _module4;

        public SystemTesterAspNetApplication(){
            InitializeComponent();
            LastLogonParametersReading += OnLastLogonParametersReading;
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        private void SystemTesterAspNetApplication_DatabaseVersionMismatch(object sender,
            DatabaseVersionMismatchEventArgs e){
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if (Debugger.IsAttached){
                e.Updater.Update();
                e.Handled = true;
            }
            else{
                string message =
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the following help topics for more detailed information:\r\n" +
                    "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm\r\n" +
                    "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm\r\n" +
                    "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/";

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null){
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        private void InitializeComponent(){
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new SystemTesterModule();
            _module4 = new SystemTesterAspNetModule();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // SystemTesterAspNetApplication
            // 
            ApplicationName = "SystemTester";
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);

            DatabaseVersionMismatch += SystemTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize) (this)).EndInit();
        }
    }
}
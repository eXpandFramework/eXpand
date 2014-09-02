using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using XpandSystemTester.Module;
using XpandSystemTester.Module.Web;

namespace XpandSystemTester.Web {
    public class XpandSystemTesterAspNetApplication : WebApplication {
        SystemModule _module1;
        SystemAspNetModule _module2;
        XpandSystemTesterModule _module3;
        XpandSystemTesterAspNetModule _module4;
        SqlConnection _sqlConnection1;
        

        public XpandSystemTesterAspNetApplication() {
            InitializeComponent();
            LastLogonParametersReading += OnLastLogonParametersReading;
        }
#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        protected override bool SupportMasterDetailMode {
            get { return true; }
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        void XpandSystemTesterAspNetApplication_DatabaseVersionMismatch(object sender,
                                                                        DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
             e.Updater.Update();
            e.Handled = true;
            if (Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
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

                if (e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        void InitializeComponent() {
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new XpandSystemTesterModule();
            _module4 = new XpandSystemTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=XpandSystemTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // XpandSystemTesterAspNetApplication
            // 
            ApplicationName = "XpandSystemTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);

            DatabaseVersionMismatch +=
                XpandSystemTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize) (this)).EndInit();
        }
    }
}
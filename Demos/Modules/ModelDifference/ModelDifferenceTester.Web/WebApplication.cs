#if !EASYTEST
using System;
using System.Diagnostics;
#endif
using System.ComponentModel;
using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using ModelDifferenceTester.Module;
using ModelDifferenceTester.Module.Web;
using Xpand.ExpressApp.ModelDifference;
using Xpand.ExpressApp.ModelDifference.Web;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Web;

namespace ModelDifferenceTester.Web {
    public class ModelDifferenceTesterAspNetApplication : XpandWebApplication {
        
        
        CloneObjectModule _cloneObjectModule1;
        ModelDifferenceAspNetModule _modelDifferenceAspNetModule1;
        ModelDifferenceModule _modelDifferenceModule1;
        SystemModule _module1;
        SystemAspNetModule _module2;
        ModelDifferenceTesterModule _module3;
        ModelDifferenceTesterAspNetModule _module4;
        SecurityModule _securityModule1;

        SqlConnection _sqlConnection1;
        XpandSecurityModule _xpandSecurityModule1;

        public ModelDifferenceTesterAspNetApplication() {
            InitializeComponent();
            DatabaseVersionMismatch += ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch;
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

        void ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch(object sender,
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
            _module3 = new ModelDifferenceTesterModule();
            _module4 = new ModelDifferenceTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            _cloneObjectModule1 = new CloneObjectModule();
            _securityModule1 = new SecurityModule();
            
            _xpandSecurityModule1 = new XpandSecurityModule();
            _modelDifferenceModule1 = new ModelDifferenceModule();
            _modelDifferenceAspNetModule1 = new ModelDifferenceAspNetModule();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
                "odelDifferenceTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // _securityStrategyComplex
            // 
            
            // 
            // _authenticationStandard
            // 

            
            // 
            // ModelDifferenceTesterAspNetApplication
            // 
            ApplicationName = "ModelDifferenceTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_cloneObjectModule1);
            Modules.Add(_securityModule1);
            Modules.Add(_xpandSecurityModule1);
            Modules.Add(_modelDifferenceModule1);
            Modules.Add(_modelDifferenceAspNetModule1);
            Modules.Add(_module4);
            ((ISupportInitialize) (this)).EndInit();
        }

        
    }
}
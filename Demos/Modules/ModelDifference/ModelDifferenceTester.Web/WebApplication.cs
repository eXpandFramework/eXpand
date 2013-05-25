using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using ModelDifferenceTester.Module;
using ModelDifferenceTester.Module.Web;
using Xpand.ExpressApp.Web;

namespace ModelDifferenceTester.Web {
    public class ModelDifferenceTesterAspNetApplication : XpandWebApplication {
        AuthenticationActiveDirectory authenticationActiveDirectory1;
        CloneObjectModule cloneObjectModule1;
        SystemModule module1;
        SystemAspNetModule module2;
        ModelDifferenceTesterModule module3;
        ModelDifferenceTesterAspNetModule module4;
        SecurityModule securityModule1;

        SecuritySimple securitySimple1;
        SqlConnection sqlConnection1;

        public ModelDifferenceTesterAspNetApplication() {
            InitializeComponent();
            DatabaseVersionMismatch+=ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProviderThreadSafe(args.ConnectionString, args.Connection);
        }

        void ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch(object sender,
                                                                            DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            } else {
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
            module1 = new SystemModule();
            module2 = new SystemAspNetModule();
            module3 = new ModelDifferenceTesterModule();
            module4 = new ModelDifferenceTesterAspNetModule();
            sqlConnection1 = new SqlConnection();
            cloneObjectModule1 = new CloneObjectModule();
            securityModule1 = new SecurityModule();

            securitySimple1 = new SecuritySimple();
            authenticationActiveDirectory1 = new AuthenticationActiveDirectory();
            ((ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
                "odelDifferenceTester";
            sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securitySimple1
            // 
            securitySimple1.Authentication = authenticationActiveDirectory1;
            securitySimple1.UserType = typeof(SimpleUser);
            // 
            // authenticationActiveDirectory1
            // 
            authenticationActiveDirectory1.CreateUserAutomatically = true;
            authenticationActiveDirectory1.LogonParametersType = null;
            // 
            // ModelDifferenceTesterAspNetApplication
            // 
            ApplicationName = "ModelDifferenceTester";
            Connection = sqlConnection1;
            Modules.Add(module1);
            Modules.Add(module2);
            Modules.Add(module3);
            Modules.Add(cloneObjectModule1);
            Modules.Add(securityModule1);

            Modules.Add(module4);
            Security = securitySimple1;
            DatabaseVersionMismatch += ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize)(this)).EndInit();
        }
    }
}
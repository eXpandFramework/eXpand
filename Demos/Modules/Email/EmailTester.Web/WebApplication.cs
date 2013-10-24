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
using EmailTester.Module;
using EmailTester.Module.BusinessObjects;
using EmailTester.Module.Web;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Web;

namespace EmailTester.Web {
    public class EmailTesterAspNetApplication : XpandWebApplication {
        AuthenticationStandard _authenticationStandard;
        SecurityStrategyComplex _securityStrategyComplex;
        CloneObjectModule cloneObjectModule1;
        EmailTesterAspNetModule EmailAspNetModule1;
        EmailTesterModule EmailModule1;
        SystemModule module1;
        SystemAspNetModule module2;
        
        
        SecurityModule securityModule1;

        SqlConnection sqlConnection1;
        XpandSecurityModule xpandSecurityModule1;

        public EmailTesterAspNetApplication() {
            InitializeComponent();
            DatabaseVersionMismatch += EmailTesterAspNetApplication_DatabaseVersionMismatch;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProviderThreadSafe(args.ConnectionString, args.Connection);
        }

        void EmailTesterAspNetApplication_DatabaseVersionMismatch(object sender,
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
            module1 = new SystemModule();
            module2 = new SystemAspNetModule();
            
            
            sqlConnection1 = new SqlConnection();
            cloneObjectModule1 = new CloneObjectModule();
            securityModule1 = new SecurityModule();
            _securityStrategyComplex = new SecurityStrategyComplex();
            _authenticationStandard = new AuthenticationStandard();
            xpandSecurityModule1 = new XpandSecurityModule();
            EmailModule1 = new EmailTesterModule();
            EmailAspNetModule1 = new EmailTesterAspNetModule();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // sqlConnection1
            // 
            sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
                "odelDifferenceTester";
            sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // _securityStrategyComplex
            // 
            _securityStrategyComplex.Authentication = _authenticationStandard;
            _securityStrategyComplex.UserType = typeof (User);
            _securityStrategyComplex.RoleType = typeof (XpandRole);
            // 
            // _authenticationStandard
            // 

            _authenticationStandard.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // EmailTesterAspNetApplication
            // 
            ApplicationName = "EmailTester";
            Connection = sqlConnection1;
            Modules.Add(module1);
            Modules.Add(module2);
            Modules.Add(cloneObjectModule1);
            Modules.Add(securityModule1);
            Modules.Add(xpandSecurityModule1);
            Modules.Add(EmailModule1);
            Modules.Add(EmailAspNetModule1);
            Security = _securityStrategyComplex;
            ((ISupportInitialize) (this)).EndInit();
        }
        
    }
}
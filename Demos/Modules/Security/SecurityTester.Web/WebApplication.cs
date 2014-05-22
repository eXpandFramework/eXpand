using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using SecurityTester.Module;
using SecurityTester.Module.Web;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Web.AuthenticationProviders;
using Xpand.Persistent.Base.General;

namespace SecurityTester.Web {
    public class SecurityTesterAspNetApplication : WebApplication, IWriteSecuredLogonParameters {
        AnonymousAuthenticationStandard _authenticationStandard1;
        SecurityStrategyComplex _securityStrategyComplex1;
        SystemModule _module1;
        SystemAspNetModule _module2;
        SecurityTesterModule _module3;
        SecurityTesterAspNetModule _module4;
        SqlConnection _sqlConnection1;

        public SecurityTesterAspNetApplication() {
            InitializeComponent();
        }
#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
        public new SettingsStorage CreateLogonParameterStoreCore() {
            return base.CreateLogonParameterStoreCore();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        void SecurityTesterAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            e.Updater.Update();
            e.Handled = true;
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
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new SecurityTesterModule();
            _module4 = new SecurityTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            _securityStrategyComplex1 = new SecurityStrategyComplex();
            _authenticationStandard1 = new AnonymousAuthenticationStandard();
            ((ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=SecurityTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // SecurityTesterAspNetApplication
            // 
            _securityStrategyComplex1.Authentication = _authenticationStandard1;
            _securityStrategyComplex1.RoleType = typeof(XpandRole);
            _securityStrategyComplex1.UserType = typeof(XpandUser);
            // 
            // authenticationStandard1
            // 
            _authenticationStandard1.LogonParametersType = typeof(AnonymousLogonParameters);
            ApplicationName = "SecurityTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);
            Security = _securityStrategyComplex1;

            DatabaseVersionMismatch += SecurityTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize)(this)).EndInit();
        }

        protected override void WriteSecuredLogonParameters() {
            var handledEventArgs = new HandledEventArgs();
            OnCustomWriteSecuredLogonParameters(handledEventArgs);
            if (!handledEventArgs.Handled)
                base.WriteSecuredLogonParameters();
        }

        public event HandledEventHandler CustomWriteSecuredLogonParameters;

        protected virtual void OnCustomWriteSecuredLogonParameters(HandledEventArgs e) {
            var handler = CustomWriteSecuredLogonParameters;
            if (handler != null) handler(this, e);
        }
    }
}
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using AuditTrailTester.Module;
using AuditTrailTester.Module.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.AuditTrail;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Security.Core;

//using DevExpress.ExpressApp.Security;

namespace AuditTrailTester.Web {
    // You can override various virtual methods and handle corresponding events to manage various aspects of your XAF application UI and behavior.
    public class AuditTrailTesterAspNetApplication : WebApplication {
        // http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
        AuditTrailModule auditTrailModule1;
        AuthenticationStandard authenticationStandard1;
        ConditionalAppearanceModule conditionalAppearanceModule1;
        LogicModule logicModule1;
        SystemModule module1;
        SystemAspNetModule module2;
        AuditTrailTesterModule module3;
        AuditTrailTesterAspNetModule module4;
        SecurityModule securityModule1;
        SecurityStrategyComplex securityStrategyComplex1;
        SqlConnection sqlConnection1;
        ValidationModule validationModule1;
        XpandAuditTrailModule xpandAuditTrailModule1;

        public AuditTrailTesterAspNetApplication() {
            InitializeComponent();
            LastLogonParametersRead += OnLastLogonParametersRead;
        }

        void OnLastLogonParametersRead(object sender, LastLogonParametersReadEventArgs lastLogonParametersReadEventArgs) {
            var parameters = ((AuthenticationStandardLogonParameters) lastLogonParametersReadEventArgs.LogonObject);
            if (string.IsNullOrEmpty(parameters.UserName))
                parameters.UserName = "User";
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProviderThreadSafe(args.ConnectionString, args.Connection);
        }

        void AuditTrailTesterAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (true) {
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
            module3 = new AuditTrailTesterModule();
            module4 = new AuditTrailTesterAspNetModule();
            sqlConnection1 = new SqlConnection();
            auditTrailModule1 = new AuditTrailModule();
            securityModule1 = new SecurityModule();
            validationModule1 = new ValidationModule();
            conditionalAppearanceModule1 = new ConditionalAppearanceModule();
            logicModule1 = new LogicModule();
            xpandAuditTrailModule1 = new XpandAuditTrailModule();
            securityStrategyComplex1 = new SecurityStrategyComplex();
            authenticationStandard1 = new AuthenticationStandard();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // sqlConnection1
            // 
            sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=A" +
                "uditTrailTester";
            sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // auditTrailModule1
            // 
            auditTrailModule1.AuditDataItemPersistentType = typeof (AuditDataItemPersistent);
            // 
            // validationModule1
            // 
            validationModule1.AllowValidationDetailsAccess = true;
            // 
            // securityStrategyComplex1
            // 
            securityStrategyComplex1.Authentication = authenticationStandard1;
            securityStrategyComplex1.RoleType = typeof (XpandRole);
            securityStrategyComplex1.UserType = typeof (SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            authenticationStandard1.LogonParametersType = typeof (AuthenticationStandardLogonParameters);
            // 
            // AuditTrailTesterAspNetApplication
            // 
            ApplicationName = "AuditTrailTester";
            Connection = sqlConnection1;
            Modules.Add(module1);
            Modules.Add(module2);
            Modules.Add(auditTrailModule1);
            Modules.Add(securityModule1);
            Modules.Add(validationModule1);
            Modules.Add(conditionalAppearanceModule1);
            Modules.Add(logicModule1);
            Modules.Add(xpandAuditTrailModule1);
            Modules.Add(module3);
            Modules.Add(module4);
            Security = securityStrategyComplex1;
            DatabaseVersionMismatch += AuditTrailTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize) (this)).EndInit();
        }
    }
}
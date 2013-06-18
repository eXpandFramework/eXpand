using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using ModelDifferenceTester.Module;
using ModelDifferenceTester.Module.Web;
using Xpand.ExpressApp.Web;

namespace ModelDifferenceTester.Web {
    public class ModelDifferenceTesterAspNetApplication : XpandWebApplication {
        AuthenticationStandard _authenticationStandard;
        CloneObjectModule cloneObjectModule1;
        SystemModule module1;
        SystemAspNetModule module2;
        ModelDifferenceTesterModule module3;
        ModelDifferenceTesterAspNetModule module4;
        SecurityModule securityModule1;

        SecurityStrategyComplex _securityStrategyComplex;
        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
        private Xpand.ExpressApp.ModelDifference.ModelDifferenceModule modelDifferenceModule1;
        private Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule modelDifferenceAspNetModule1;
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
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module3 = new ModelDifferenceTester.Module.ModelDifferenceTesterModule();
            this.module4 = new ModelDifferenceTester.Module.Web.ModelDifferenceTesterAspNetModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this._securityStrategyComplex = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this._authenticationStandard = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this.xpandSecurityModule1 = new Xpand.ExpressApp.Security.XpandSecurityModule();
            this.modelDifferenceModule1 = new Xpand.ExpressApp.ModelDifference.ModelDifferenceModule();
            this.modelDifferenceAspNetModule1 = new Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
    "odelDifferenceTester";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // _securityStrategyComplex
            // 
            this._securityStrategyComplex.Authentication = this._authenticationStandard;
            this._securityStrategyComplex.UserType = typeof(SecuritySystemUser);
            this._securityStrategyComplex.RoleType = typeof(SecuritySystemRole);
            // 
            // _authenticationStandard
            // 

            this._authenticationStandard.LogonParametersType = typeof(AuthenticationStandardLogonParameters);
            // 
            // ModelDifferenceTesterAspNetApplication
            // 
            this.ApplicationName = "ModelDifferenceTester";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.modelDifferenceAspNetModule1);
            this.Modules.Add(this.module4);
            this.Security = this._securityStrategyComplex;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}
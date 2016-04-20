using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using XtraDashboardTester.Module.Web;

//using DevExpress.ExpressApp.Security;

namespace XtraDashboardTester.Web{
    // You can override various virtual methods and handle corresponding events to manage various aspects of your XAF application UI and behavior.
    public class XtraDashboardTesterAspNetApplication : WebApplication, IWriteSecuredLogonParameters {
        // http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
        private SystemModule _module1;
        private SystemAspNetModule _module2;

        private XtraDashboardTesterAspNetModule _module4;
        private SecurityStrategyComplex _securityStrategyComplex1;
        private AuthenticationStandard _authenticationStandard1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
        private Xpand.ExpressApp.Dashboard.DashboardModule dashboardModule1;
        private DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule validationAspNetModule1;
        private Xpand.ExpressApp.XtraDashboard.Web.XtraDashboardWebModule xtraDashboardWebModule1;
        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        private DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule scriptRecorderAspNetModule1;
        private Module.ModuleModule moduleModule1;
        private SqlConnection _sqlConnection1;

        public XtraDashboardTesterAspNetApplication(){
            InitializeComponent();
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        private void XtraDashboardTesterAspNetApplication_DatabaseVersionMismatch(object sender,
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
            this._module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this._module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this._securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this._authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this._module4 = new XtraDashboardTester.Module.Web.XtraDashboardTesterAspNetModule();
            this._sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.dashboardModule1 = new Xpand.ExpressApp.Dashboard.DashboardModule();
            this.validationAspNetModule1 = new DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule();
            this.xtraDashboardWebModule1 = new Xpand.ExpressApp.XtraDashboard.Web.XtraDashboardWebModule();
            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.scriptRecorderAspNetModule1 = new DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule();
            this.moduleModule1 = new XtraDashboardTester.Module.ModuleModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // _securityStrategyComplex1
            // 
            this._securityStrategyComplex1.Authentication = this._authenticationStandard1;
            this._securityStrategyComplex1.RoleType = typeof(Xpand.ExpressApp.Security.Core.XpandRole);
            this._securityStrategyComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // _authenticationStandard1
            // 
            this._authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // _sqlConnection1
            // 
            this._sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=X" +
    "traDashboardTester";
            this._sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            this.validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // XtraDashboardTesterAspNetApplication
            // 
            this.ApplicationName = "XtraDashboardTester";
            this.Connection = this._sqlConnection1;
            this.Modules.Add(this._module1);
            this.Modules.Add(this._module2);
            this.Modules.Add(this.validationModule1);
            this.Modules.Add(this.dashboardModule1);
            this.Modules.Add(this.validationAspNetModule1);
            this.Modules.Add(this.xtraDashboardWebModule1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.scriptRecorderAspNetModule1);
            this.Modules.Add(this.moduleModule1);
            this.Modules.Add(this._module4);
            this.Security = this._securityStrategyComplex1;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ScriptRecorder;
using DevExpress.ExpressApp.ScriptRecorder.Web;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.Web;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.XtraDashboard.Web;
using Xpand.Persistent.Base.General;
using XtraDashboardTester.Module;
using XtraDashboardTester.Module.Web;

//using DevExpress.ExpressApp.Security;

namespace XtraDashboardTester.Web{
    // You can override various virtual methods and handle corresponding events to manage various aspects of your XAF application UI and behavior.
    public class XtraDashboardTesterAspNetApplication : WebApplication, IWriteSecuredLogonParameters{
        // http://documentation.devexpress.com/#Xaf/DevExpressExpressAppWebWebApplicationMembersTopicAll
        private SystemModule _module1;
        private SystemAspNetModule _module2;

        private XtraDashboardTesterAspNetModule _module4;
        private SecurityStrategyComplex _securityStrategyComplex1;
        private AuthenticationStandard _authenticationStandard1;
        private ValidationModule _validationModule1;
        private DashboardModule _dashboardModule1;
        private ValidationAspNetModule _validationAspNetModule1;
        private XtraDashboardWebModule _xtraDashboardWebModule1;
        private ScriptRecorderModuleBase _scriptRecorderModuleBase1;
        private ScriptRecorderAspNetModule _scriptRecorderAspNetModule1;
        private ModuleModule _moduleModule1;
        private SqlConnection _sqlConnection1;

        public XtraDashboardTesterAspNetApplication(){
            InitializeComponent();
            LastLogonParametersReading += OnLastLogonParametersReading;
            DatabaseVersionMismatch += XtraDashboardTesterAspNetApplication_DatabaseVersionMismatch;
        }

        private void OnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e){
            if (string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))){
                e.SettingsStorage.SaveOption("", "UserName", "Admin");
            }
        }

#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif

        protected override void WriteSecuredLogonParameters(){
            var handledEventArgs = new HandledEventArgs();
            OnCustomWriteSecuredLogonParameters(handledEventArgs);
            if (!handledEventArgs.Handled)
                base.WriteSecuredLogonParameters();
        }

        public event HandledEventHandler CustomWriteSecuredLogonParameters;

        protected virtual void OnCustomWriteSecuredLogonParameters(HandledEventArgs e){
            var handler = CustomWriteSecuredLogonParameters;
            handler?.Invoke(this, e);
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
                var message =
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the following help topics for more detailed information:\r\n" +
                    "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm\r\n" +
                    "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm\r\n" +
                    "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/";

                if (e.CompatibilityError?.Exception != null){
                    message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
                }
                throw new InvalidOperationException(message);
            }
#endif
        }

        private void InitializeComponent(){
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _securityStrategyComplex1 = new SecurityStrategyComplex();
            _authenticationStandard1 = new AuthenticationStandard();
            _module4 = new XtraDashboardTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            _validationModule1 = new ValidationModule();
            _dashboardModule1 = new DashboardModule();
            _validationAspNetModule1 = new ValidationAspNetModule();
            _xtraDashboardWebModule1 = new XtraDashboardWebModule();
            _scriptRecorderModuleBase1 = new ScriptRecorderModuleBase();
            _scriptRecorderAspNetModule1 = new ScriptRecorderAspNetModule();
            _moduleModule1 = new ModuleModule();
            ((ISupportInitialize) this).BeginInit();
            // 
            // _securityStrategyComplex1
            // 
            _securityStrategyComplex1.Authentication = _authenticationStandard1;
            _securityStrategyComplex1.RoleType = typeof(XpandRole);
            _securityStrategyComplex1.UserType = typeof(SecuritySystemUser);
            // 
            // _authenticationStandard1
            // 
            _authenticationStandard1.LogonParametersType = typeof(AuthenticationStandardLogonParameters);
            // 
            // _sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=X" +
                "traDashboardTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // validationModule1
            // 
            _validationModule1.AllowValidationDetailsAccess = true;
            _validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // XtraDashboardTesterAspNetApplication
            // 
            ApplicationName = "XtraDashboardTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_validationModule1);
            Modules.Add(_dashboardModule1);
            Modules.Add(_validationAspNetModule1);
            Modules.Add(_xtraDashboardWebModule1);
            Modules.Add(_scriptRecorderModuleBase1);
            Modules.Add(_scriptRecorderAspNetModule1);
            Modules.Add(_moduleModule1);
            Modules.Add(_module4);
            Security = _securityStrategyComplex1;
            ((ISupportInitialize) this).EndInit();
        }
    }
}
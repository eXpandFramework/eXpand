#if !EASYTEST
using System;
using System.Diagnostics;
#endif
using System.ComponentModel;
using System.Data.SqlClient;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using ModelArtifactStateTester.Module;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelArtifactState;
using Xpand.ExpressApp.Validation;

namespace ModelArtifactStateTester.Web {
    public class ModelArtifactStateTesterAspNetApplication : WebApplication {
        private SystemModule _module1;
        private SystemAspNetModule _module2;
        private ModelArtifactStateTesterModule _module3;
        private ValidationModule _validationModule1;
        private XpandValidationModule _xpandValidationModule1;
        private SecurityModule _securityModule1;
        private ConditionalAppearanceModule _conditionalAppearanceModule1;
        private LogicModule _logicModule1;
        private ModelArtifactStateModule _modelArtifactStateModule1;

        private SqlConnection _sqlConnection1;

        public ModelArtifactStateTesterAspNetApplication() {
            InitializeComponent();
            LastLogonParametersReading+=OnLastLogonParametersReading;
            DatabaseVersionMismatch+=ModelArtifactStateTesterAspNetApplication_DatabaseVersionMismatch;
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
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
        }

        private void ModelArtifactStateTesterAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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
                string message = "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
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

        private void InitializeComponent() {
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new ModelArtifactStateTesterModule();
            _sqlConnection1 = new SqlConnection();
            _validationModule1 = new ValidationModule();
            _xpandValidationModule1 = new XpandValidationModule();
            _securityModule1 = new SecurityModule();
            _conditionalAppearanceModule1 = new ConditionalAppearanceModule();
            _logicModule1 = new LogicModule();
            _modelArtifactStateModule1 = new ModelArtifactStateModule();
            ((ISupportInitialize) (this)).BeginInit();
            // 
            // _sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
                "odelArtifactStateTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // validationModule1
            // 
            _validationModule1.AllowValidationDetailsAccess = true;
            _validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // ModelArtifactStateTesterAspNetApplication
            // 
            ApplicationName = "ModelArtifactStateTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_validationModule1);
            Modules.Add(_xpandValidationModule1);
            Modules.Add(_securityModule1);
            Modules.Add(_conditionalAppearanceModule1);
            Modules.Add(_logicModule1);
            Modules.Add(_modelArtifactStateModule1);
            Modules.Add(_module3);
            ((ISupportInitialize) (this)).EndInit();

        }
    }
}

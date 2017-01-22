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
using EmailTester.Module;
using EmailTester.Module.Web;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;

namespace EmailTester.Web {
    public class EmailTesterAspNetApplication : WebApplication {
        AuthenticationStandard _authenticationStandard;
        SecurityStrategyComplex _securityStrategyComplex;
        CloneObjectModule _cloneObjectModule1;
        EmailTesterAspNetModule _emailAspNetModule1;
        EmailTesterModule _emailModule1;
        SystemModule _module1;
        SystemAspNetModule _module2;
        
        
        SecurityModule _securityModule1;

        SqlConnection _sqlConnection1;
        XpandSecurityModule _xpandSecurityModule1;

        public EmailTesterAspNetApplication() {
            InitializeComponent();
            DatabaseVersionMismatch += EmailTesterAspNetApplication_DatabaseVersionMismatch;
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(new ConnectionStringDataStoreProvider(args.ConnectionString).CachedInstance(),  true);
        }

        void EmailTesterAspNetApplication_DatabaseVersionMismatch(object sender,
                                                                            DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            e.Updater.Update();
            e.Handled = true;
            
#endif
        }

        void InitializeComponent() {
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            
            
            _sqlConnection1 = new SqlConnection();
            _cloneObjectModule1 = new CloneObjectModule();
            _securityModule1 = new SecurityModule();
            _securityStrategyComplex = new SecurityStrategyComplex();
            _authenticationStandard = new AuthenticationStandard();
            _xpandSecurityModule1 = new XpandSecurityModule();
            _emailModule1 = new EmailTesterModule();
            _emailAspNetModule1 = new EmailTesterAspNetModule();
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
            _securityStrategyComplex.Authentication = _authenticationStandard;
            _securityStrategyComplex.UserType = typeof (XpandUser);
            _securityStrategyComplex.RoleType = typeof (XpandRole);
            // 
            // _authenticationStandard
            // 

            _authenticationStandard.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // EmailTesterAspNetApplication
            // 
            ApplicationName = "EmailTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_cloneObjectModule1);
            Modules.Add(_securityModule1);
            Modules.Add(_xpandSecurityModule1);
            Modules.Add(_emailModule1);
            Modules.Add(_emailAspNetModule1);
            Security = _securityStrategyComplex;
            ((ISupportInitialize) (this)).EndInit();
        }

        
    }
}
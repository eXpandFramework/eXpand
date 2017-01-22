#if !EASYTEST
using System;
using System.Diagnostics;
#endif
using System.ComponentModel;
using System.Data.SqlClient;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using SecurityTester.Module;
using SecurityTester.Module.Web;
using Xpand.Persistent.Base.General;

namespace SecurityTester.Web {
    public class SecurityTesterAspNetApplication : WebApplication {
        SystemModule _module1;
        SystemAspNetModule _module2;
        SecurityTesterModule _module3;
        SecurityTesterAspNetModule _module4;
        SqlConnection _sqlConnection1;

        public SecurityTesterAspNetApplication() {
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(new ConnectionStringDataStoreProvider(args.ConnectionString).CachedInstance(), true);
        }

        

        void SecurityTesterAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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
            _module3 = new SecurityTesterModule();
            _module4 = new SecurityTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            
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

            // 
            // authenticationStandard1
            // 
            
            ApplicationName = "SecurityTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);
            

            DatabaseVersionMismatch += SecurityTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize)(this)).EndInit();
        }


       
    }
}
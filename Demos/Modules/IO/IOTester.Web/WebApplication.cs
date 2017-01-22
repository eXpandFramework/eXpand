#if !EASYTEST
using System;
using System.Diagnostics;
#endif
using System.ComponentModel;
using System.Data.SqlClient;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using IOTester.Module;
using IOTester.Module.Web;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;

namespace IOTester.Web {
    public class IOTesterAspNetApplication : WebApplication {
        SystemModule _module1;
        SystemAspNetModule _module2;
        IOTesterModule _module3;
        IOTesterAspNetModule _module4;
        SqlConnection _sqlConnection1;

        public IOTesterAspNetApplication() {
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

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(args.ConnectionString));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider());
            if (this.GetEasyTestParameter("NorthWind"))
                args.ObjectSpaceProviders.Add(new WorldCreatorObjectSpaceProvider());
        }

        void IOTesterAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
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
            _module3 = new IOTesterModule();
            _module4 = new IOTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            ((ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=IOTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // IOTesterAspNetApplication
            // 
            ApplicationName = "IOTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);

            DatabaseVersionMismatch += IOTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize)(this)).EndInit();
        }
    }
}
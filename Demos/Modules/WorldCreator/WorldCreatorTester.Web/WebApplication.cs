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
using WorldCreatorTester.Module;
using WorldCreatorTester.Module.Web;
using Xpand.Persistent.Base.General;

namespace WorldCreatorTester.Web {
    public class WorldCreatorTesterAspNetApplication : WebApplication {
        SystemModule _module1;
        SystemAspNetModule _module2;
        WorldCreatorTesterModule _module3;
        WorldCreatorTesterAspNetModule _module4;
        SqlConnection _sqlConnection1;

        public WorldCreatorTesterAspNetApplication() {
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
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs e) {
            var easyTestParameter = this.GetEasyTestParameter("DBMapper");
            e.ObjectSpaceProviders.Add(easyTestParameter
                ? new XpandObjectSpaceProvider(new MultiDataStoreProvider(e.ConnectionString), Security)
                : new XPObjectSpaceProvider(e.ConnectionString));
            e.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        void WorldCreatorTesterAspNetApplication_DatabaseVersionMismatch(object sender,
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
            _module3 = new WorldCreatorTesterModule();
            _module4 = new WorldCreatorTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            ((ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            _sqlConnection1.ConnectionString =
                @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=WorldCreatorTester";
            _sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // WorldCreatorTesterAspNetApplication
            // 
            ApplicationName = "WorldCreatorTester";
            CheckCompatibilityType=CheckCompatibilityType.DatabaseSchema;
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);

            DatabaseVersionMismatch += WorldCreatorTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize)(this)).EndInit();
        }


        
    }
}
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
using ModelDifferenceTester.Module;
using ModelDifferenceTester.Module.FunctionalTests;
using ModelDifferenceTester.Module.Web;
using Xpand.ExpressApp.ModelDifference;
using Xpand.ExpressApp.ModelDifference.Web;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;

namespace ModelDifferenceTester.Web {
    public class ModelDifferenceTesterAspNetApplication : WebApplication {
        
        
        CloneObjectModule _cloneObjectModule1;
        ModelDifferenceAspNetModule _modelDifferenceAspNetModule1;
        ModelDifferenceModule _modelDifferenceModule1;
        SystemModule _module1;
        SystemAspNetModule _module2;
        ModelDifferenceTesterModule _module3;
        ModelDifferenceTesterAspNetModule _module4;
        SecurityModule _securityModule1;

        SqlConnection _sqlConnection1;
        XpandSecurityModule _xpandSecurityModule1;

        public ModelDifferenceTesterAspNetApplication() {
            InitializeComponent();
            this.ProjectSetup();
            DatabaseVersionMismatch += ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch;
            LastLogonParametersReading += OnLastLogonParametersReading;
        }


        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(args.ConnectionString));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider());
            if (this.GetEasyTestParameter(EasyTestParameters.WCModel))
                args.ObjectSpaceProviders.Add(new WorldCreatorObjectSpaceProvider());
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

        void ModelDifferenceTesterAspNetApplication_DatabaseVersionMismatch(object sender,
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
            _module3 = new ModelDifferenceTesterModule();
            _module4 = new ModelDifferenceTesterAspNetModule();
            _sqlConnection1 = new SqlConnection();
            _cloneObjectModule1 = new CloneObjectModule();
            _securityModule1 = new SecurityModule();
            
            _xpandSecurityModule1 = new XpandSecurityModule();
            _modelDifferenceModule1 = new ModelDifferenceModule();
            _modelDifferenceAspNetModule1 = new ModelDifferenceAspNetModule();
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
            
            // 
            // _authenticationStandard
            // 

            
            // 
            // ModelDifferenceTesterAspNetApplication
            // 
            ApplicationName = "ModelDifferenceTester";
            Connection = _sqlConnection1;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_cloneObjectModule1);
            Modules.Add(_securityModule1);
            Modules.Add(_xpandSecurityModule1);
            Modules.Add(_modelDifferenceModule1);
            Modules.Add(_modelDifferenceAspNetModule1);
            Modules.Add(_module4);
            ((ISupportInitialize) (this)).EndInit();
        }


        
    }
}
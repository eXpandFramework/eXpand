using System;
using System.ComponentModel;
using System.Data;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using ExcelImporterTester.Module;
using ExcelImporterTester.Module.Web;

namespace ExcelImporterTester.Web{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/DevExpressExpressAppWebWebApplicationMembersTopicAll.aspx
    public class ExcelImporterTesterAspNetApplication : WebApplication{
        private SystemModule _module1;
        private SystemAspNetModule _module2;
        private ExcelImporterTesterModule _module3;
        private ExcelImporterTesterAspNetModule _module4;

        public ExcelImporterTesterAspNetApplication(){
            InitializeComponent();
            InitializeDefaults();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider =
                new XPObjectSpaceProvider(GetDataStoreProvider(args.ConnectionString, args.Connection), true);
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }

        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, IDbConnection connection){
            var application = HttpContext.Current != null ? HttpContext.Current.Application : null;
            IXpoDataStoreProvider dataStoreProvider;
            if (application?["DataStoreProvider"] != null){
                dataStoreProvider = application["DataStoreProvider"] as IXpoDataStoreProvider;
            }
            else{
                dataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, true);
                if (application != null) application["DataStoreProvider"] = dataStoreProvider;
            }

            return dataStoreProvider;
        }

        private void ExcelImporterTesterAspNetApplication_DatabaseVersionMismatch(object sender,
            DatabaseVersionMismatchEventArgs e){
            e.Updater.Update();
            e.Handled = true;

        }

        #region Component Designer generated code

        private void InitializeComponent(){
            _module1 = new SystemModule();
            _module2 = new SystemAspNetModule();
            _module3 = new ExcelImporterTesterModule();
            _module4 = new ExcelImporterTesterAspNetModule();
            ((ISupportInitialize) this).BeginInit();
            // 
            // ExcelImporterTesterAspNetApplication
            // 
            ApplicationName = "ExcelImporterTester";
            CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema;
            Modules.Add(_module1);
            Modules.Add(_module2);
            Modules.Add(_module3);
            Modules.Add(_module4);
            DatabaseVersionMismatch += ExcelImporterTesterAspNetApplication_DatabaseVersionMismatch;
            ((ISupportInitialize) this).EndInit();
        }
        

        #endregion
        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)

        static ExcelImporterTesterAspNetApplication(){
            EnableMultipleBrowserTabsSupport = true;
            ASPxGridListEditor.AllowFilterControlHierarchy = true;
            ASPxGridListEditor.MaxFilterControlHierarchyDepth = 3;
            ASPxCriteriaPropertyEditor.AllowFilterControlHierarchyDefault = true;
            ASPxCriteriaPropertyEditor.MaxHierarchyDepthDefault = 3;
            PasswordCryptographer.EnableRfc2898 = true;
            PasswordCryptographer.SupportLegacySha512 = false;
        }

        private void InitializeDefaults(){
            LinkNewObjectToParentImmediately = false;
            OptimizedControllersCreation = true;
        }

        #endregion
    }
}
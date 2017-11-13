using System;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.Mobile;
using System.Collections.Generic;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Internal;

namespace MainDemo.Mobile {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/DevExpressExpressAppWebWebApplicationMembersTopicAll.aspx
    public partial class MainDemoMobileApplication : MobileApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Mobile.SystemModule.SystemMobileModule module2;
        private MainDemo.Module.MainDemoModule module3;
        private MainDemo.Module.Mobile.MainDemoMobileModule module4;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityStrategyComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
        private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule;
        private DevExpress.ExpressApp.ConditionalAppearance.Mobile.ConditionalAppearanceMobileModule conditionalAppearanceMobileModule;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule;
        private DevExpress.ExpressApp.Validation.Mobile.ValidationMobileModule validationMobile;
        private DevExpress.ExpressApp.ReportsV2.ReportsModuleV2 reportsModuleV21;
        private DevExpress.ExpressApp.ReportsV2.Mobile.ReportsMobileModuleV2 reportsMobileModuleV21;
        private DevExpress.ExpressApp.FileAttachments.Mobile.FileAttachmentsMobileModule fileAttachmentsMobileModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule businessClassLibraryCustomizationModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.Maps.Mobile.MapsMobileModule mapsMobileModule1;
        private DevExpress.ExpressApp.Localization.Mobile.LocalizationMobileModule localizationMobileModule1;
        

        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
        static MainDemoMobileApplication() {
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
        }
        private void InitializeDefaults() {
            LinkNewObjectToParentImmediately = false;
        }
        #endregion
        public MainDemoMobileApplication() {
            SecurityAdapterHelper.Enable();
            string siteMode = ConfigurationManager.AppSettings["SiteMode"];
            if(siteMode != null && siteMode.ToLower() == "true") {
                ConnectionString = ConfigurationManager.ConnectionStrings["MainDemoMobileConnectionString"].ConnectionString;
            }
            else {
                if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                    ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
                else if(string.IsNullOrEmpty(ConnectionString) && Connection == null) {
                    var connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                    if(connectionStringSettings != null) {
                        ConnectionString = DbEngineDetector.PatchConnectionString(connectionStringSettings.ConnectionString);
                    }
                }
            }

            InitializeComponent();
            InitializeDefaults();

            if(System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }


        }
        protected override void SetLogonParametersForUIBuilder(object logonParameters) {
            base.SetLogonParametersForUIBuilder(logonParameters);
            ((AuthenticationStandardLogonParameters)logonParameters).UserName = "Sam";
            ((AuthenticationStandardLogonParameters)logonParameters).Password = "";
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new SecuredObjectSpaceProvider((SecurityStrategyComplex)Security, GetDataStoreProvider(args.ConnectionString, args.Connection), true);
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, System.Data.IDbConnection connection) {
            System.Web.HttpApplicationState application = (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Application : null;
            IXpoDataStoreProvider dataStoreProvider = null;
            if(application != null && application["DataStoreProvider"] != null) {
                dataStoreProvider = application["DataStoreProvider"] as IXpoDataStoreProvider;
            }
            else {
                if(!String.IsNullOrEmpty(connectionString)) {
                    dataStoreProvider = new ConnectionStringDataStoreProvider(connectionString);
                }
                else if(connection != null) {
                    dataStoreProvider = new ConnectionDataStoreProvider(connection);
                }

                if(application != null) {
                    application["DataStoreProvider"] = dataStoreProvider;
                }
            }
            return dataStoreProvider;
        }
        private void MainDemoMobileApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
            e.Updater.Update();
            e.Handled = true;
        }
        private void MainDemoMobileApplication_LastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if(string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }
        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Mobile.SystemModule.SystemMobileModule();
            this.module3 = new MainDemo.Module.MainDemoModule();
            this.module4 = new MainDemo.Module.Mobile.MainDemoMobileModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.securityStrategyComplex1.SupportNavigationPermissionsForTypes = false;
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this.auditTrailModule = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            this.cloneObjectModule = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.conditionalAppearanceModule = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.conditionalAppearanceMobileModule = new DevExpress.ExpressApp.ConditionalAppearance.Mobile.ConditionalAppearanceMobileModule();
            this.validationModule = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.validationMobile = new DevExpress.ExpressApp.Validation.Mobile.ValidationMobileModule();
            this.reportsModuleV21 = new DevExpress.ExpressApp.ReportsV2.ReportsModuleV2();
            this.reportsMobileModuleV21 = new DevExpress.ExpressApp.ReportsV2.Mobile.ReportsMobileModuleV2();
            this.fileAttachmentsMobileModule1 = new DevExpress.ExpressApp.FileAttachments.Mobile.FileAttachmentsMobileModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.mapsMobileModule1 = new DevExpress.ExpressApp.Maps.Mobile.MapsMobileModule();
            this.businessClassLibraryCustomizationModule1 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole);
            this.securityStrategyComplex1.UserType = typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            //
            // auditTrailModule
            //
            this.auditTrailModule.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
            // 
            // validationModule
            // 
            this.validationModule.AllowValidationDetailsAccess = true;
            this.validationModule.IgnoreWarningAndInformationRules = false;
            // 
            // reportsModuleV21
            // 
            this.reportsModuleV21.EnableInplaceReports = true;
            this.reportsModuleV21.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
            //
            // mapsMobileModule1
            //
            this.mapsMobileModule1.GoogleApiKey = "AIzaSyDk2m6n8ICK7FSmTHBLlapAWF3epiDdkHE";
            this.mapsMobileModule1.SetMapsEditorsAsDefault = false;
            // 
            // pivotChartModuleBase1
            // 

            this.localizationMobileModule1 = new DevExpress.ExpressApp.Localization.Mobile.LocalizationMobileModule();
            this.pivotChartModuleBase1.DataAccessMode = DevExpress.ExpressApp.CollectionSourceDataAccessMode.Client;
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // MainDemoMobileApplication
            // 
            this.ApplicationName = "MainDemo";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.businessClassLibraryCustomizationModule1);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.validationModule);
            this.Modules.Add(this.conditionalAppearanceModule);
            this.Modules.Add(this.reportsModuleV21);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.auditTrailModule);
            this.Modules.Add(this.cloneObjectModule);
            this.Modules.Add(this.validationMobile);
            this.Modules.Add(this.reportsMobileModuleV21);
            this.Modules.Add(this.conditionalAppearanceMobileModule);
            this.Modules.Add(this.fileAttachmentsMobileModule1);
            this.Modules.Add(this.mapsMobileModule1);
            this.Modules.Add(this.localizationMobileModule1);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.MainDemoMobileApplication_DatabaseVersionMismatch);
            this.LastLogonParametersReading += MainDemoMobileApplication_LastLogonParametersReading;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}

using System;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Web;

namespace FeatureCenter.Web.ApplicationCode {
    public partial class FeatureCenterAspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;

        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.Security.SecurityComplex securityComplex1;
        private XpandAuthenticationStandard authenticationStandard1;
        private Module.FeatureCenterModule featureCenterModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        private DevExpress.ExpressApp.ConditionalFormatting.ConditionalFormattingModule conditionalFormattingModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;
        private Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule conditionalControllerStateModule1;
        private Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule conditionalActionStateModule1;
        private Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule modelArtifactStateModule1;
        private Xpand.ExpressApp.ModelDifference.ModelDifferenceModule modelDifferenceModule1;
        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
        private Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule xpandViewVariantsModule1;
        private Xpand.ExpressApp.WorldCreator.WorldCreatorModule worldCreatorModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private Xpand.ExpressApp.IO.IOModule ioModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private Xpand.ExpressApp.PivotChart.XpandPivotChartModule xpandPivotChartModule1;
        private Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule filterDataStoreModule1;

        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        private Xpand.ExpressApp.WorldCreator.SqlDBMapper.WorldCreatorSqlDBMapperModule worldCreatorSqlDBMapperModule1;
        private Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule conditionalDetailViewModule1;
        private Xpand.ExpressApp.MemberLevelSecurity.MemberLevelSecurityModule memberLevelSecurityModule1;
        private Module.Web.FeatureCenterAspNetModule featureCenterAspNetModule1;

        private Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule exceptionHandlingWebModule1;

        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule xpandPivotChartAspNetModule1;
        private Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule thumbnailWebModule1;
        private Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule nCarouselWebModule1;
        private Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule xpandSystemAspNetModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule1;
        private DevExpress.ExpressApp.ConditionalEditorState.ConditionalEditorStateModuleBase conditionalEditorStateModuleBase1;
        private DevExpress.ExpressApp.ConditionalEditorState.Web.ConditionalEditorStateAspNetModule conditionalEditorStateAspNetModule1;
        private Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule worldCreatorWebModule1;
        private DevExpress.ExpressApp.Validation.ValidationModule module5;

        public FeatureCenterAspNetApplication() {
            InitializeComponent();
        }

        private void AspNetApplicationDatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (true) {
                e.Updater.ForceUpdateDatabase = true;
                e.Updater.Update();
                e.Handled = true;
            } else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
                    "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " +
                    "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
            }
#endif
        }

        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module5 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityComplex();
            this.authenticationStandard1 = new XpandAuthenticationStandard();
            this.featureCenterModule1 = new FeatureCenter.Module.FeatureCenterModule();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.conditionalFormattingModule1 = new DevExpress.ExpressApp.ConditionalFormatting.ConditionalFormattingModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.xpandSystemModule1 = new Xpand.ExpressApp.SystemModule.XpandSystemModule();
            this.logicModule1 = new Xpand.ExpressApp.Logic.LogicModule();
            this.xpandValidationModule1 = new Xpand.ExpressApp.Validation.XpandValidationModule();
            this.conditionalControllerStateModule1 = new Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule();
            this.conditionalActionStateModule1 = new Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule();
            this.modelArtifactStateModule1 = new Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule();
            this.modelDifferenceModule1 = new Xpand.ExpressApp.ModelDifference.ModelDifferenceModule();
            this.xpandSecurityModule1 = new Xpand.ExpressApp.Security.XpandSecurityModule();
            this.xpandViewVariantsModule1 = new Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule();
            this.worldCreatorModule1 = new Xpand.ExpressApp.WorldCreator.WorldCreatorModule();
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.ioModule1 = new Xpand.ExpressApp.IO.IOModule();

            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.xpandPivotChartModule1 = new Xpand.ExpressApp.PivotChart.XpandPivotChartModule();
            this.filterDataStoreModule1 = new Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule();

            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.worldCreatorSqlDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.SqlDBMapper.WorldCreatorSqlDBMapperModule();
            this.conditionalDetailViewModule1 = new Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule();
            this.memberLevelSecurityModule1 = new Xpand.ExpressApp.MemberLevelSecurity.MemberLevelSecurityModule();
            this.featureCenterAspNetModule1 = new FeatureCenter.Module.Web.FeatureCenterAspNetModule();

            this.exceptionHandlingWebModule1 = new Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule();

            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.xpandPivotChartAspNetModule1 = new Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule();
            this.thumbnailWebModule1 = new Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule();
            this.nCarouselWebModule1 = new Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule();
            this.xpandSystemAspNetModule1 = new Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule();
            this.fileAttachmentsAspNetModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.conditionalEditorStateModuleBase1 = new DevExpress.ExpressApp.ConditionalEditorState.ConditionalEditorStateModuleBase();
            this.conditionalEditorStateAspNetModule1 = new DevExpress.ExpressApp.ConditionalEditorState.Web.ConditionalEditorStateAspNetModule();
            this.worldCreatorWebModule1 = new Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=XpandFeatureCenter;Integrated Security=SSPI;P" +
                "ooling=false";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityComplex1
            // 
            this.securityComplex1.Authentication = this.authenticationStandard1;
            this.securityComplex1.RoleType = typeof(DevExpress.Persistent.BaseImpl.Role);
            this.securityComplex1.UserType = typeof(DevExpress.Persistent.BaseImpl.User);
            // 
            // authenticationStandard1
            // 

            // 
            // viewVariantsModule1
            // 
            this.viewVariantsModule1.GenerateVariantsNode = true;
            this.viewVariantsModule1.ShowAdditionalNavigation = false;
            // 
            // pivotChartModuleBase1
            // 
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // FeatureCenterAspNetApplication
            // 
            this.ApplicationName = "FeatureCenter";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.conditionalFormattingModule1);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.xpandSystemModule1);
            this.Modules.Add(this.logicModule1);
            this.Modules.Add(this.xpandValidationModule1);
            this.Modules.Add(this.conditionalControllerStateModule1);
            this.Modules.Add(this.conditionalActionStateModule1);
            this.Modules.Add(this.modelArtifactStateModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.xpandViewVariantsModule1);
            this.Modules.Add(this.worldCreatorModule1);
            this.Modules.Add(this.treeListEditorsModuleBase1);
            this.Modules.Add(this.ioModule1);

            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.xpandPivotChartModule1);
            this.Modules.Add(this.filterDataStoreModule1);

            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.worldCreatorSqlDBMapperModule1);
            this.Modules.Add(this.conditionalDetailViewModule1);
            this.Modules.Add(this.memberLevelSecurityModule1);
            this.Modules.Add(this.featureCenterModule1);

            this.Modules.Add(this.exceptionHandlingWebModule1);

            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.xpandPivotChartAspNetModule1);
            this.Modules.Add(this.thumbnailWebModule1);
            this.Modules.Add(this.nCarouselWebModule1);
            this.Modules.Add(this.xpandSystemAspNetModule1);
            this.Modules.Add(this.fileAttachmentsAspNetModule1);
            this.Modules.Add(this.conditionalEditorStateModuleBase1);
            this.Modules.Add(this.conditionalEditorStateAspNetModule1);
            this.Modules.Add(this.worldCreatorWebModule1);
            this.Modules.Add(this.featureCenterAspNetModule1);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.AspNetApplicationDatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}

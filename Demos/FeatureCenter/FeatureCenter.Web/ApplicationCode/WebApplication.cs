using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Web;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Web.ApplicationCode {
    public partial class FeatureCenterAspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;

        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityComplex1;
        private XpandAuthenticationStandard authenticationStandard1;
        private Module.FeatureCenterModule featureCenterModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;
        
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
        

        private Module.Web.FeatureCenterAspNetModule featureCenterAspNetModule1;

        private Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule exceptionHandlingWebModule1;

        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule xpandPivotChartAspNetModule1;
        private Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule thumbnailWebModule1;
        private Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule nCarouselWebModule1;
        private Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule xpandSystemAspNetModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule1;

        private Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule worldCreatorWebModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.Kpi.KpiModule kpiModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule additionalViewControlsModule1;
        private Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule worldCreatorDBMapperModule1;
        private Xpand.ExpressApp.JobScheduler.JobSchedulerModule jobSchedulerModule1;
        private Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule jobSchedulerJobsModule1;
        private Xpand.ExpressApp.ImportWizard.ImportWizardModule importWizardModule1;
        private DevExpress.ExpressApp.StateMachine.StateMachineModule stateMachineModule1;
        private DevExpress.ExpressApp.Workflow.WorkflowModule workflowModule1;
        private Xpand.ExpressApp.Workflow.XpandWorkFlowModule xpandWorkFlowModule1;
        private Xpand.ExpressApp.StateMachine.XpandStateMachineModule xpandStateMachineModule1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule htmlPropertyEditorAspNetModule1;
        private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        private DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule reportsAspNetModule1;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule treeListEditorsAspNetModule1;
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule schedulerAspNetModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule additionalViewControlsProviderAspNetModule1;
        private Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule modelDifferenceAspNetModule1;
        private Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule xpandTreeListEditorsModule1;
        private Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule xpandTreeListEditorsAspNetModule1;
        private Xpand.ExpressApp.IO.Web.IOAspNetModule ioAspNetModule1;
        private Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule filterDataStoreAspNetModule1;
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
                if (this.DropDatabaseOnVersionMissmatch() > 0)
                    Exit();
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
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.authenticationStandard1 = new Xpand.ExpressApp.Security.AuthenticationProviders.XpandAuthenticationStandard();
            this.featureCenterModule1 = new FeatureCenter.Module.FeatureCenterModule();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.xpandSystemModule1 = new Xpand.ExpressApp.SystemModule.XpandSystemModule();
            this.logicModule1 = new Xpand.ExpressApp.Logic.LogicModule();
            this.xpandValidationModule1 = new Xpand.ExpressApp.Validation.XpandValidationModule();
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
            this.featureCenterAspNetModule1 = new FeatureCenter.Module.Web.FeatureCenterAspNetModule();
            this.exceptionHandlingWebModule1 = new Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule();
            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.xpandPivotChartAspNetModule1 = new Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule();
            this.thumbnailWebModule1 = new Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule();
            this.nCarouselWebModule1 = new Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule();
            this.xpandSystemAspNetModule1 = new Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule();
            this.fileAttachmentsAspNetModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.worldCreatorWebModule1 = new Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.kpiModule1 = new DevExpress.ExpressApp.Kpi.KpiModule();
            this.additionalViewControlsModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule();
            this.worldCreatorDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule();
            this.jobSchedulerModule1 = new Xpand.ExpressApp.JobScheduler.JobSchedulerModule();
            this.jobSchedulerJobsModule1 = new Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule();
            this.importWizardModule1 = new Xpand.ExpressApp.ImportWizard.ImportWizardModule();
            this.stateMachineModule1 = new DevExpress.ExpressApp.StateMachine.StateMachineModule();
            this.workflowModule1 = new DevExpress.ExpressApp.Workflow.WorkflowModule();
            this.xpandWorkFlowModule1 = new Xpand.ExpressApp.Workflow.XpandWorkFlowModule();
            this.xpandStateMachineModule1 = new Xpand.ExpressApp.StateMachine.XpandStateMachineModule();
            this.htmlPropertyEditorAspNetModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule();
            this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            this.reportsAspNetModule1 = new DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule();
            this.treeListEditorsAspNetModule1 = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerAspNetModule1 = new DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule();
            this.additionalViewControlsProviderAspNetModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule();
            this.modelDifferenceAspNetModule1 = new Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule();
            this.xpandTreeListEditorsModule1 = new Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule();
            this.xpandTreeListEditorsAspNetModule1 = new Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule();
            this.ioAspNetModule1 = new Xpand.ExpressApp.IO.Web.IOAspNetModule();
            this.filterDataStoreAspNetModule1 = new Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule();
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
            this.securityComplex1.RoleType = typeof(Xpand.ExpressApp.Security.Core.XpandRole);
            this.securityComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(Xpand.ExpressApp.Security.AuthenticationProviders.XpandLogonParameters);
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
            // stateMachineModule1
            // 
            this.stateMachineModule1.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
            // 
            // workflowModule1
            // 
            this.workflowModule1.RunningWorkflowInstanceInfoType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoRunningWorkflowInstanceInfo);
            this.workflowModule1.StartWorkflowRequestType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoStartWorkflowRequest);
            this.workflowModule1.UserActivityVersionType = typeof(DevExpress.ExpressApp.Workflow.Versioning.XpoUserActivityVersion);
            this.workflowModule1.WorkflowControlCommandRequestType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowInstanceControlCommandRequest);
            this.workflowModule1.WorkflowDefinitionType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowDefinition);
            this.workflowModule1.WorkflowInstanceKeyType = typeof(DevExpress.Workflow.Xpo.XpoInstanceKey);
            this.workflowModule1.WorkflowInstanceType = typeof(DevExpress.Workflow.Xpo.XpoWorkflowInstance);
            // 
            // reportsModule1
            // 
            this.reportsModule1.EnableInplaceReports = true;
            this.reportsModule1.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportData);
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
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.xpandSystemModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.logicModule1);
            this.Modules.Add(this.xpandValidationModule1);
            this.Modules.Add(this.modelArtifactStateModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.xpandViewVariantsModule1);
            this.Modules.Add(this.worldCreatorModule1);
            this.Modules.Add(this.treeListEditorsModuleBase1);
            this.Modules.Add(this.ioModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.xpandPivotChartModule1);
            this.Modules.Add(this.filterDataStoreModule1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.kpiModule1);
            this.Modules.Add(this.additionalViewControlsModule1);
            this.Modules.Add(this.worldCreatorDBMapperModule1);
            this.Modules.Add(this.jobSchedulerModule1);
            this.Modules.Add(this.jobSchedulerJobsModule1);
            this.Modules.Add(this.importWizardModule1);
            this.Modules.Add(this.stateMachineModule1);
            this.Modules.Add(this.workflowModule1);
            this.Modules.Add(this.xpandWorkFlowModule1);
            this.Modules.Add(this.xpandStateMachineModule1);
            this.Modules.Add(this.featureCenterModule1);
            this.Modules.Add(this.exceptionHandlingWebModule1);
            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.xpandPivotChartAspNetModule1);
            this.Modules.Add(this.thumbnailWebModule1);
            this.Modules.Add(this.nCarouselWebModule1);
            this.Modules.Add(this.xpandSystemAspNetModule1);
            this.Modules.Add(this.fileAttachmentsAspNetModule1);
            this.Modules.Add(this.worldCreatorWebModule1);
            this.Modules.Add(this.htmlPropertyEditorAspNetModule1);
            this.Modules.Add(this.reportsModule1);
            this.Modules.Add(this.reportsAspNetModule1);
            this.Modules.Add(this.treeListEditorsAspNetModule1);
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerAspNetModule1);
            this.Modules.Add(this.additionalViewControlsProviderAspNetModule1);
            this.Modules.Add(this.modelDifferenceAspNetModule1);
            this.Modules.Add(this.xpandTreeListEditorsModule1);
            this.Modules.Add(this.xpandTreeListEditorsAspNetModule1);
            this.Modules.Add(this.ioAspNetModule1);
            this.Modules.Add(this.filterDataStoreAspNetModule1);
            this.Modules.Add(this.featureCenterAspNetModule1);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.AspNetApplicationDatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}

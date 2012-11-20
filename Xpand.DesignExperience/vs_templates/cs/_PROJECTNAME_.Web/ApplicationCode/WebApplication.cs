using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;



namespace $projectsuffix$.Web {
    public partial class $projectsuffix$AspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;
        private $projectsuffix$.Module.$projectsuffix$Module module3;
        private $projectsuffix$.Module.Web.$projectsuffix$AspNetModule module4;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;

        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule1;

        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule reportsAspNetModule1;


        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule treeListEditorsAspNetModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;
        private Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule conditionalControllerStateModule1;
        private Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule conditionalActionStateModule1;
        private Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule modelArtifactStateModule1;
        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
        private Xpand.ExpressApp.ModelDifference.ModelDifferenceModule modelDifferenceModule1;
        private Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule xpandViewVariantsModule1;
        private Xpand.ExpressApp.WorldCreator.WorldCreatorModule worldCreatorModule1;
        private Xpand.ExpressApp.IO.IOModule ioModule1;

        private Xpand.ExpressApp.PivotChart.XpandPivotChartModule xpandPivotChartModule1;
        private Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule xpandTreeListEditorsModule1;
        private Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule filterDataStoreModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule additionalViewControlsModule1;
        private Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule worldCreatorDBMapperModule1;
        private Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule conditionalDetailViewModule1;

        private Xpand.ExpressApp.JobScheduler.JobSchedulerModule jobSchedulerModule1;
        private Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule jobSchedulerJobsModule1;
        private Xpand.ExpressApp.ImportWizard.ImportWizardModule importWizardModule1;
        private DevExpress.ExpressApp.Workflow.WorkflowModule workflowModule1;
        private Xpand.ExpressApp.Workflow.XpandWorkFlowModule xpandWorkFlowModule1;
        private DevExpress.ExpressApp.StateMachine.StateMachineModule stateMachineModule1;
        private Xpand.ExpressApp.StateMachine.XpandStateMachineModule xpandStateMachineModule1;
        private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        private Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule xpandSystemAspNetModule1;
        private Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule modelDifferenceAspNetModule1;
        private Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule worldCreatorWebModule1;
        private Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule exceptionHandlingWebModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule additionalViewControlsProviderAspNetModule1;
        private Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule xpandPivotChartAspNetModule1;
        private Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule nCarouselWebModule1;
        private Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule thumbnailWebModule1;
        private Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule filterDataStoreAspNetModule1;
        private Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule xpandTreeListEditorsAspNetModule1;
        private Xpand.ExpressApp.IO.Web.IOAspNetModule ioAspNetModule1;
        private Xpand.ExpressApp.Validation.Web.XpandValidationWebModule xpandValidationWebModule1;
        private SecurityStrategyComplex securityStrategyComplex1;
        private XpandAuthenticationStandard authenticationStandard1;



        private DevExpress.ExpressApp.Validation.ValidationModule module5;

        public $projectsuffix$AspNetApplication() {
            InitializeComponent();
            LastLogonParametersRead += OnLastLogonParametersRead;
        }

        void OnLastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            AuthenticationStandardLogonParameters logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if (logonParameters != null) {
                if (String.IsNullOrEmpty(logonParameters.UserName)) {
                    logonParameters.UserName = "Admin";
                }
            }
        }
        private void $projectsuffix$AspNetApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
			e.Updater.Update();
			e.Handled = true;
#else
            if (System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            } else {
                throw new InvalidOperationException(
                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
                    "The automatic update is disabled, because the application was started without debugging.\r\n" +
                    "You should start the application under Visual Studio, or modify the " +
                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
                    "or manually create a database using the 'DBUpdater' tool.");
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
            this.auditTrailModule1 = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.fileAttachmentsAspNetModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.reportsAspNetModule1 = new DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule();
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.treeListEditorsAspNetModule1 = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.module3 = new $projectsuffix$.Module.$projectsuffix$Module();
            this.module4 = new $projectsuffix$.Module.Web.$projectsuffix$AspNetModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.xpandSystemModule1 = new Xpand.ExpressApp.SystemModule.XpandSystemModule();
            this.logicModule1 = new Xpand.ExpressApp.Logic.LogicModule();
            this.xpandValidationModule1 = new Xpand.ExpressApp.Validation.XpandValidationModule();
            this.conditionalControllerStateModule1 = new Xpand.ExpressApp.ConditionalControllerState.ConditionalControllerStateModule();
            this.conditionalActionStateModule1 = new Xpand.ExpressApp.ConditionalActionState.ConditionalActionStateModule();
            this.modelArtifactStateModule1 = new Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule();
            this.xpandSecurityModule1 = new Xpand.ExpressApp.Security.XpandSecurityModule();
            this.modelDifferenceModule1 = new Xpand.ExpressApp.ModelDifference.ModelDifferenceModule();
            this.xpandViewVariantsModule1 = new Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule();
            this.worldCreatorModule1 = new Xpand.ExpressApp.WorldCreator.WorldCreatorModule();
            this.ioModule1 = new Xpand.ExpressApp.IO.IOModule();

            this.xpandPivotChartModule1 = new Xpand.ExpressApp.PivotChart.XpandPivotChartModule();
            this.xpandTreeListEditorsModule1 = new Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule();
            this.filterDataStoreModule1 = new Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule();
            this.additionalViewControlsModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule();
            this.worldCreatorDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule();
            this.conditionalDetailViewModule1 = new Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule();

            this.jobSchedulerModule1 = new Xpand.ExpressApp.JobScheduler.JobSchedulerModule();
            this.jobSchedulerJobsModule1 = new Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule();
            this.importWizardModule1 = new Xpand.ExpressApp.ImportWizard.ImportWizardModule();
            this.workflowModule1 = new DevExpress.ExpressApp.Workflow.WorkflowModule();
            this.xpandWorkFlowModule1 = new Xpand.ExpressApp.Workflow.XpandWorkFlowModule();
            this.stateMachineModule1 = new DevExpress.ExpressApp.StateMachine.StateMachineModule();
            this.xpandStateMachineModule1 = new Xpand.ExpressApp.StateMachine.XpandStateMachineModule();
            this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            this.xpandSystemAspNetModule1 = new Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule();
            this.modelDifferenceAspNetModule1 = new Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule();
            this.worldCreatorWebModule1 = new Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule();
            this.exceptionHandlingWebModule1 = new Xpand.ExpressApp.ExceptionHandling.Web.ExceptionHandlingWebModule();
            this.additionalViewControlsProviderAspNetModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule();
            this.xpandPivotChartAspNetModule1 = new Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule();
            this.nCarouselWebModule1 = new Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule();
            this.thumbnailWebModule1 = new Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule();
            this.filterDataStoreAspNetModule1 = new Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule();
            this.xpandTreeListEditorsAspNetModule1 = new Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule();
            this.ioAspNetModule1 = new Xpand.ExpressApp.IO.Web.IOAspNetModule();
            this.xpandValidationWebModule1 = new Xpand.ExpressApp.Validation.Web.XpandValidationWebModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.authenticationStandard1 = new XpandAuthenticationStandard();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=$projectsuffix$;Integrated Security=SSPI;Pooling=" +
    "false";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // auditTrailModule1
            // 
            this.auditTrailModule1.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
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
            // stateMachineModule1
            // 
            this.stateMachineModule1.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
            // 
            // reportsModule1
            // 
            this.reportsModule1.EnableInplaceReports = true;
            this.reportsModule1.ReportDataType = typeof(DevExpress.ExpressApp.Reports.ReportData);
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(Xpand.ExpressApp.Security.Core.XpandRole);
            this.securityStrategyComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // $projectsuffix$AspNetApplication
            // 
            this.ApplicationName = "$projectsuffix$";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.auditTrailModule1);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.xpandSystemModule1);
            this.Modules.Add(this.logicModule1);
            this.Modules.Add(this.xpandValidationModule1);
            this.Modules.Add(this.conditionalControllerStateModule1);
            this.Modules.Add(this.conditionalActionStateModule1);
            this.Modules.Add(this.modelArtifactStateModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.xpandViewVariantsModule1);
            this.Modules.Add(this.worldCreatorModule1);
            this.Modules.Add(this.treeListEditorsModuleBase1);
            this.Modules.Add(this.ioModule1);

            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.xpandPivotChartModule1);
            this.Modules.Add(this.xpandTreeListEditorsModule1);
            this.Modules.Add(this.filterDataStoreModule1);
            this.Modules.Add(this.additionalViewControlsModule1);
            this.Modules.Add(this.worldCreatorDBMapperModule1);
            this.Modules.Add(this.conditionalDetailViewModule1);

            this.Modules.Add(this.jobSchedulerModule1);
            this.Modules.Add(this.jobSchedulerJobsModule1);
            this.Modules.Add(this.importWizardModule1);
            this.Modules.Add(this.workflowModule1);
            this.Modules.Add(this.xpandWorkFlowModule1);
            this.Modules.Add(this.stateMachineModule1);
            this.Modules.Add(this.xpandStateMachineModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.fileAttachmentsAspNetModule1);
            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.reportsModule1);
            this.Modules.Add(this.reportsAspNetModule1);
            this.Modules.Add(this.treeListEditorsAspNetModule1);
            this.Modules.Add(this.xpandSystemAspNetModule1);
            this.Modules.Add(this.modelDifferenceAspNetModule1);
            this.Modules.Add(this.worldCreatorWebModule1);
            this.Modules.Add(this.exceptionHandlingWebModule1);
            this.Modules.Add(this.additionalViewControlsProviderAspNetModule1);
            this.Modules.Add(this.xpandPivotChartAspNetModule1);
            this.Modules.Add(this.nCarouselWebModule1);
            this.Modules.Add(this.thumbnailWebModule1);
            this.Modules.Add(this.filterDataStoreAspNetModule1);
            this.Modules.Add(this.xpandTreeListEditorsAspNetModule1);
            this.Modules.Add(this.ioAspNetModule1);
            this.Modules.Add(this.xpandValidationWebModule1);
            this.Modules.Add(this.module4);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.$projectsuffix$AspNetApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}



using FeatureCenter.Module;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;

namespace FeatureCenter.Win {
    partial class FeatureCenterWindowsFormsApplication {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule();
            this.module3 = new FeatureCenter.Module.FeatureCenterModule();
            this.module4 = new FeatureCenter.Module.Win.FeatureCenterWindowsFormsModule();
            this.module5 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.module7 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.fileAttachmentsWindowsFormsModule1 = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
            this.htmlPropertyEditorWindowsFormsModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.pivotChartWindowsFormsModule1 = new DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule();
            this.reportsWindowsFormsModule1 = new DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule();
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerWindowsFormsModule1 = new DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule();
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.treeListEditorsWindowsFormsModule1 = new DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule();
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
            this.ioModule1 = new Xpand.ExpressApp.IO.IOModule();
            this.masterDetailModule1 = new Xpand.ExpressApp.MasterDetail.MasterDetailModule();
            this.xpandPivotChartModule1 = new Xpand.ExpressApp.PivotChart.XpandPivotChartModule();
            this.filterDataStoreModule1 = new Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule();
            this.additionalViewControlsModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule();
            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.worldCreatorSqlDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.SqlDBMapper.WorldCreatorSqlDBMapperModule();
            this.conditionalDetailViewModule1 = new Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule();
            this.memberLevelSecurityModule1 = new Xpand.ExpressApp.MemberLevelSecurity.MemberLevelSecurityModule();
            this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            this.modelDifferenceWindowsFormsModule1 = new Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule();
            this.xpandTreeListEditorsWinModule1 = new Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule();
            this.xpandSystemWindowsFormsModule1 = new Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule();
            this.wizardUIWindowsFormsModule1 = new Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule();
            this.worldCreatorWinModule1 = new Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule();
            this.ioWinModule1 = new Xpand.ExpressApp.IO.Win.IOWinModule();
            this.exceptionHandlingWinModule1 = new Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule();
            this.filterDataStoreWindowsFormsModule1 = new Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule();
            this.additionalViewControlsProviderWindowsFormsModule1 = new Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule();
            this.xpandPivotChartWinModule1 = new Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule();
            this.logicWindowsModule1 = new Xpand.ExpressApp.Logic.Win.LogicWindowsModule();
            this.masterDetailWindowsModule1 = new Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule();
            this.scriptRecorderWindowsFormsModule1 = new DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule();
            this.memberLevelSecurityModuleWin1 = new Xpand.ExpressApp.MemberLevelSecurity.Win.MemberLevelSecurityModuleWin();
            this.jobSchedulerModule1 = new Xpand.ExpressApp.JobScheduler.JobSchedulerModule();
            this.jobSchedulerJobsModule1 = new Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule();
            this.xpandTreeListEditorsModule1 = new Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.kpiModule1 = new DevExpress.ExpressApp.Kpi.KpiModule();
            this.importWizardModule1 = new Xpand.ExpressApp.ImportWizard.ImportWizardModule();
            this.stateMachineModule1 = new DevExpress.ExpressApp.StateMachine.StateMachineModule();
            this.importWizardWindowsFormsModule1 = new Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule();
            this.workflowModule1 = new DevExpress.ExpressApp.Workflow.WorkflowModule();
            this.workflowWindowsFormsModule1 = new DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule();
            this.xpandStateMachineModule1 = new Xpand.ExpressApp.StateMachine.XpandStateMachineModule();
            this.xpandWorkFlowModule2 = new Xpand.ExpressApp.Workflow.XpandWorkFlowModule();
            this.xpandValidationWinModule1 = new Xpand.ExpressApp.Validation.Win.XpandValidationWinModule();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityComplex();
            this.authenticationStandard1 = new XpandAuthenticationStandard();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=FeatureCenter;Integrated Security=SSPI;Poolin" +
                "g=false";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
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
            // reportsModule1
            // 
            this.reportsModule1.EnableInplaceReports = true;
            this.reportsModule1.ReportDataType = typeof(DevExpress.ExpressApp.Reports.ReportData);
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
            // securityComplex1
            // 
            this.securityComplex1.Authentication = this.authenticationStandard1;
            this.securityComplex1.RoleType = typeof(DevExpress.Persistent.BaseImpl.Role);
            this.securityComplex1.UserType = typeof(DevExpress.Persistent.BaseImpl.User);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // FeatureCenterWindowsFormsApplication
            // 
            this.ApplicationName = "FeatureCenter";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.securityModule1);
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
            this.Modules.Add(this.masterDetailModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.xpandPivotChartModule1);
            this.Modules.Add(this.filterDataStoreModule1);
            this.Modules.Add(this.additionalViewControlsModule1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.worldCreatorSqlDBMapperModule1);
            this.Modules.Add(this.conditionalDetailViewModule1);
            this.Modules.Add(this.memberLevelSecurityModule1);
            this.Modules.Add(this.jobSchedulerModule1);
            this.Modules.Add(this.jobSchedulerJobsModule1);
            this.Modules.Add(this.kpiModule1);
            this.Modules.Add(this.importWizardModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.stateMachineModule1);
            this.Modules.Add(this.workflowModule1);
            this.Modules.Add(this.xpandStateMachineModule1);
            this.Modules.Add(this.xpandWorkFlowModule2);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.fileAttachmentsWindowsFormsModule1);
            this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule1);
            this.Modules.Add(this.pivotChartWindowsFormsModule1);
            this.Modules.Add(this.reportsModule1);
            this.Modules.Add(this.reportsWindowsFormsModule1);
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerWindowsFormsModule1);
            this.Modules.Add(this.treeListEditorsWindowsFormsModule1);
            this.Modules.Add(this.module7);
            this.Modules.Add(this.xpandSystemWindowsFormsModule1);
            this.Modules.Add(this.modelDifferenceWindowsFormsModule1);
            this.Modules.Add(this.xpandTreeListEditorsModule1);
            this.Modules.Add(this.xpandTreeListEditorsWinModule1);
            this.Modules.Add(this.wizardUIWindowsFormsModule1);
            this.Modules.Add(this.worldCreatorWinModule1);
            this.Modules.Add(this.ioWinModule1);
            this.Modules.Add(this.exceptionHandlingWinModule1);
            this.Modules.Add(this.filterDataStoreWindowsFormsModule1);
            this.Modules.Add(this.additionalViewControlsProviderWindowsFormsModule1);
            this.Modules.Add(this.xpandPivotChartWinModule1);
            this.Modules.Add(this.logicWindowsModule1);
            this.Modules.Add(this.masterDetailWindowsModule1);
            this.Modules.Add(this.scriptRecorderWindowsFormsModule1);
            this.Modules.Add(this.memberLevelSecurityModuleWin1);
            this.Modules.Add(this.importWizardWindowsFormsModule1);
            this.Modules.Add(this.workflowWindowsFormsModule1);
            this.Modules.Add(this.xpandValidationWinModule1);
            this.Modules.Add(this.module4);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.FeatureCenterWindowsFormsApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion


        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private FeatureCenter.Module.FeatureCenterModule module3;
        private FeatureCenter.Module.Win.FeatureCenterWindowsFormsModule module4;
        private DevExpress.ExpressApp.Validation.ValidationModule module5;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule module7;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private System.Data.SqlClient.SqlConnection sqlConnection1;

        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;

        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;


        private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule fileAttachmentsWindowsFormsModule1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule pivotChartWindowsFormsModule1;

        private DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule reportsWindowsFormsModule1;

        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule schedulerWindowsFormsModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule treeListEditorsWindowsFormsModule1;
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
        private Xpand.ExpressApp.IO.IOModule ioModule1;
        private Xpand.ExpressApp.MasterDetail.MasterDetailModule masterDetailModule1;
        private Xpand.ExpressApp.PivotChart.XpandPivotChartModule xpandPivotChartModule1;
        private Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule filterDataStoreModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule additionalViewControlsModule1;
        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        private Xpand.ExpressApp.WorldCreator.SqlDBMapper.WorldCreatorSqlDBMapperModule worldCreatorSqlDBMapperModule1;
        private Xpand.ExpressApp.ConditionalDetailViews.ConditionalDetailViewModule conditionalDetailViewModule1;
        private Xpand.ExpressApp.MemberLevelSecurity.MemberLevelSecurityModule memberLevelSecurityModule1;
        private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        private Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule modelDifferenceWindowsFormsModule1;
        private Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule xpandTreeListEditorsWinModule1;
        private Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule xpandSystemWindowsFormsModule1;
        private Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule wizardUIWindowsFormsModule1;
        private Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule worldCreatorWinModule1;
        private Xpand.ExpressApp.IO.Win.IOWinModule ioWinModule1;
        private Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule exceptionHandlingWinModule1;
        private Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule filterDataStoreWindowsFormsModule1;
        private Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule additionalViewControlsProviderWindowsFormsModule1;
        private Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule xpandPivotChartWinModule1;
        private Xpand.ExpressApp.Logic.Win.LogicWindowsModule logicWindowsModule1;
        private Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule masterDetailWindowsModule1;
        private DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule scriptRecorderWindowsFormsModule1;
        private Xpand.ExpressApp.MemberLevelSecurity.Win.MemberLevelSecurityModuleWin memberLevelSecurityModuleWin1;
        private Xpand.ExpressApp.JobScheduler.JobSchedulerModule jobSchedulerModule1;
        private Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule jobSchedulerJobsModule1;
        private Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule xpandTreeListEditorsModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.Kpi.KpiModule kpiModule1;
        private Xpand.ExpressApp.ImportWizard.ImportWizardModule importWizardModule1;
        private DevExpress.ExpressApp.StateMachine.StateMachineModule stateMachineModule1;
        private Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule importWizardWindowsFormsModule1;
        private DevExpress.ExpressApp.Workflow.WorkflowModule workflowModule1;
        private DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule workflowWindowsFormsModule1;
        private Xpand.ExpressApp.StateMachine.XpandStateMachineModule xpandStateMachineModule1;


        private Xpand.ExpressApp.Workflow.XpandWorkFlowModule xpandWorkFlowModule2;
        private Xpand.ExpressApp.Validation.Win.XpandValidationWinModule xpandValidationWinModule1;
        private DevExpress.ExpressApp.Security.SecurityComplex securityComplex1;
        private XpandAuthenticationStandard authenticationStandard1;
    }
}

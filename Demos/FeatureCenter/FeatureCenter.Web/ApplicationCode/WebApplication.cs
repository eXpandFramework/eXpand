using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Web;
using Xpand.Persistent.Base.General;

namespace FeatureCenter.Web.ApplicationCode {
    public partial class FeatureCenterAspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule _module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule _module2;

        private SecurityModule _securityModule1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule _module6;
        private System.Data.SqlClient.SqlConnection _sqlConnection1;
        private SecurityStrategyComplex _securityComplex1;
        private XpandAuthenticationStandard _authenticationStandard1;
        private Module.FeatureCenterModule _featureCenterModule1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule _cloneObjectModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule _viewVariantsModule1;
        
        
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;

        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        

        private Module.Web.FeatureCenterAspNetModule featureCenterAspNetModule1;


        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsAspNetModule1;

        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.Kpi.KpiModule kpiModule1;
        
        private DevExpress.ExpressApp.Workflow.WorkflowModule workflowModule1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule htmlPropertyEditorAspNetModule1;
        private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        private DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule reportsAspNetModule1;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule treeListEditorsAspNetModule1;
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule schedulerAspNetModule1;
        private DevExpress.ExpressApp.StateMachine.StateMachineModule stateMachineModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase2;
        private DevExpress.ExpressApp.Validation.ValidationModule module5;

        public FeatureCenterAspNetApplication() {
            InitializeComponent();
        }
#if EASYTEST
        protected override string GetUserCultureName() {
            return "en-US";
        }
#endif
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
            this._module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this._module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module5 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this._module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this._securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this._sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this._securityComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this._authenticationStandard1 = new Xpand.ExpressApp.Security.AuthenticationProviders.XpandAuthenticationStandard();
            this._featureCenterModule1 = new FeatureCenter.Module.FeatureCenterModule();
            this._cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this._viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.featureCenterAspNetModule1 = new FeatureCenter.Module.Web.FeatureCenterAspNetModule();
            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.fileAttachmentsAspNetModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.kpiModule1 = new DevExpress.ExpressApp.Kpi.KpiModule();
            this.workflowModule1 = new DevExpress.ExpressApp.Workflow.WorkflowModule();
            this.htmlPropertyEditorAspNetModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule();
            this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            this.reportsAspNetModule1 = new DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule();
            this.treeListEditorsAspNetModule1 = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerAspNetModule1 = new DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule();
            this.stateMachineModule1 = new DevExpress.ExpressApp.StateMachine.StateMachineModule();
            this.treeListEditorsModuleBase2 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            this.module5.IgnoreWarningAndInformationRules = false;
            // 
            // securityModule1
            // 
            this._securityModule1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // sqlConnection1
            // 
            this._sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=XpandFeatureCenter;Integrated Security=SSPI;P" +
    "ooling=false";
            this._sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityComplex1
            // 
            this._securityComplex1.Authentication = this._authenticationStandard1;
            this._securityComplex1.RoleType = typeof(Xpand.ExpressApp.Security.Core.XpandRole);
            this._securityComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this._authenticationStandard1.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // viewVariantsModule1
            // 
            this._viewVariantsModule1.FrameVariantsEngine = null;
            this._viewVariantsModule1.VariantsProvider = null;
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
            // reportsModule1
            // 
            this.reportsModule1.EnableInplaceReports = true;
            this.reportsModule1.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportData);
            // 
            // stateMachineModule1
            // 
            this.stateMachineModule1.StateMachineStorageType = typeof(DevExpress.ExpressApp.StateMachine.Xpo.XpoStateMachine);
            // 
            // FeatureCenterAspNetApplication
            // 
            this.ApplicationName = "FeatureCenter";
            this.Connection = this._sqlConnection1;
            this.Modules.Add(this._module1);
            this.Modules.Add(this._module2);
            this.Modules.Add(this.module5);
            this.Modules.Add(this._module6);
            this.Modules.Add(this._securityModule1);
            this.Modules.Add(this._cloneObjectModule1);
            this.Modules.Add(this._viewVariantsModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.kpiModule1);
            this.Modules.Add(this.workflowModule1);
            this.Modules.Add(this.stateMachineModule1);
            this.Modules.Add(this._featureCenterModule1);
            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.fileAttachmentsAspNetModule1);
            this.Modules.Add(this.htmlPropertyEditorAspNetModule1);
            this.Modules.Add(this.reportsModule1);
            this.Modules.Add(this.reportsAspNetModule1);
            this.Modules.Add(this.treeListEditorsModuleBase2);
            this.Modules.Add(this.treeListEditorsAspNetModule1);
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerAspNetModule1);
            this.Modules.Add(this.featureCenterAspNetModule1);
            this.Security = this._securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.AspNetApplicationDatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}

namespace EFDemo.Module {
    partial class EFDemoModule {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
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
            // 
			// EFDemoModule
            // 
			this.Description = "EFDemo module";
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Reports.ReportsModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.PivotChartModuleBase));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));

            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Dashboard.DashboardModule));
            //			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.JobSchedulerModule));
            //			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.XpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.StateMachine.XpandStateMachineModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Workflow.XpandWorkFlowModule));
            //			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Reports.XpandReportsModule));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Address));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Analysis));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Country));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Event));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileAttachment));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileData));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.HCategory));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Note));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Organization));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Party));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Person));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.PhoneNumber));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Priority));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.ReportData));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Resource));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Role));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.State));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Task));
			this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.User));
		}

        #endregion
    }
}

namespace WorkflowDemo.Module.Win
{
    partial class WorkflowDemoWindowsFormsModule
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // WorkflowDemoWindowsFormsModule
            // 
            this.RequiredModuleTypes.Add(typeof(WorkflowDemoModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Chart.Win.XpandChartWinModule));
//			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Win.IOWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotGrid.Win.XpandPivotGridWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Reports.Win.XpandReportsWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Scheduler.Win.XpandSchedulerWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.Win.XpandSecurityWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Win.XpandValidationWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule));
//			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule));
			this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.XtraDashboard.Win.DashboardWindowsFormsModule));
        }

        #endregion
    }
}

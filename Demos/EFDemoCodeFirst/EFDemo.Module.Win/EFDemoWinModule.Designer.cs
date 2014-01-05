using DevExpress.ExpressApp.Win.Templates;
namespace EFDemo.Module.Win {
	partial class EFDemoWinModule {
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
            this.AdditionalExportedTypes.Add(typeof(DevExpress.ExpressApp.Validation.ResultsHighlightController));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.ExpressApp.Validation.AllContextsView.DisplayableValidationResultItem));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.ExpressApp.Validation.AllContextsView.ContextValidationResult));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.ExpressApp.Validation.AllContextsView.ValidationResults));
            this.Description = "EFDemo Win module";
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(EFDemo.Module.EFDemoModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Chart.Win.XpandChartWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Win.IOWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule));
            //            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotGrid.Win.XpandPivotGridWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Reports.Win.XpandReportsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.Win.XpandSecurityWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Win.XpandValidationWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));

            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.MainFormTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.DetailViewFormTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.NestedFrameTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.LookupControlTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.PopupFormTemplateLocalizer));
		}
		#endregion
	}
}

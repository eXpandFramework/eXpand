namespace MainDemo.Module.Win {
	partial class MainDemoWinModule {
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
            // MainDemoWinModule
            // 
            this.Description = "MainDemo Win module";
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(MainDemo.Module.MainDemoModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.MainFormTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.DetailViewFormTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.NestedFrameTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.LookupControlTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(DevExpress.ExpressApp.Win.Templates.PopupFormTemplateLocalizer));
            this.ResourcesExportedToModel.Add(typeof(Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));
            this.ResourcesExportedToModel.Add(typeof(Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule));
		}
		#endregion
	}
}

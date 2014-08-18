namespace EFDemo.Module.Web {
	partial class EFDemoWebModule {
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
			// EFDemoWebModule
			// 
			this.Description = "EFDemo Web module";
			this.RequiredModuleTypes.Add(typeof(EFDemo.Module.EFDemoModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule));
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Web.IOAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule));
            //            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Web.XpandValidationWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule));
		}

		#endregion
	}
}

namespace EFDemo.Win {
	partial class EFDemoWinApplication {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void DisposeCore() {
			if((components != null)) {
				components.Dispose();
			}
			base.DisposeCore();
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.systemModule1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
			this.winSystemModule1 = new DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule();
			this.efDemoWinModule1 = new EFDemo.Module.Win.EFDemoWinModule();
			this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
			this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
			this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
			this.efDemoModule1 = new EFDemo.Module.EFDemoModule();
			this.validationWinModule1 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
			this.fileAttachmentsWinModule1 = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
			this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
			this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
			this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
			this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
			this.pivotChartWindowsFormsModule1 = new DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule();
			this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
			this.schedulerWindowsFormsModule1 = new DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule();
			this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
			this.reportsWinModule1 = new DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule();
			this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
			this.htmlPropertyEditorWindowsFormsModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
			this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
			this.scriptRecorderWindowsFormsModule1 = new DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// reportsModule1
			// 
			this.reportsModule1.EnableInplaceReports = true;
			this.reportsModule1.ReportDataType = typeof(EFDemo.Module.Data.ReportData);
			// 
			// securityComplex1
			// 
			this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
			this.securityStrategyComplex1.RoleType = typeof(EFDemo.Module.Data.Role);
			this.securityStrategyComplex1.UserType = typeof(EFDemo.Module.Data.User);
			// 
			// authenticationStandard1
			// 
			this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
			this.authenticationStandard1.UserType = typeof(EFDemo.Module.Data.User);
			// 
			// sqlConnection1
			// 
			this.sqlConnection1.ConnectionString = @"Integrated Security=True;Pooling=false;MultipleActiveResultSets=True;Data Source=.\SQLExpress;Initial Catalog=EFDemo_v12.2";
			this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
			// 
			// EFDemoWinApplication
			// 
			this.ApplicationName = "EFDemo";
			this.Connection = this.sqlConnection1;
			this.Modules.Add(this.systemModule1);
			this.Modules.Add(this.winSystemModule1);
			this.Modules.Add(this.viewVariantsModule1);
			this.Modules.Add(this.validationModule1);
			this.Modules.Add(this.securityModule1);
			this.Modules.Add(this.conditionalAppearanceModule1);
			this.Modules.Add(this.efDemoModule1);
			this.Modules.Add(this.efDemoWinModule1);
			this.Modules.Add(this.validationWinModule1);
			this.Modules.Add(this.fileAttachmentsWinModule1);
			this.Modules.Add(this.pivotChartModuleBase1);
			this.Modules.Add(this.pivotChartWindowsFormsModule1);
			this.Modules.Add(this.schedulerModuleBase1);
			this.Modules.Add(this.schedulerWindowsFormsModule1);
			this.Modules.Add(this.reportsModule1);
			this.Modules.Add(this.reportsWinModule1);
			this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule1);
			this.Modules.Add(this.scriptRecorderModuleBase1);
			this.Modules.Add(this.scriptRecorderWindowsFormsModule1);
			this.Security = this.securityStrategyComplex1;
			this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.EFDemoWinApplication_DatabaseVersionMismatch);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}

		#endregion

		private DevExpress.ExpressApp.SystemModule.SystemModule systemModule1;
		private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule winSystemModule1;
		private EFDemo.Module.Win.EFDemoWinModule efDemoWinModule1;
		private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
		private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
		private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
		private EFDemo.Module.EFDemoModule efDemoModule1;
		private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule validationWinModule1;
		private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule fileAttachmentsWinModule1;
		private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
		private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityStrategyComplex1;
		private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
		private System.Data.SqlClient.SqlConnection sqlConnection1;
		private DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule pivotChartWindowsFormsModule1;
		private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
		private DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule schedulerWindowsFormsModule1;
		private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
		private DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule reportsWinModule1;
		private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
		private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule1;
		private DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule scriptRecorderWindowsFormsModule1;
		private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
	}
}

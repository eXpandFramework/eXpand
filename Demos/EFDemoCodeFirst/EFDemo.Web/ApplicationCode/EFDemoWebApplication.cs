using System;
using System.Data.Common;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Security;

using EFDemo.Module;
using EFDemo.Module.Data;
using Xpand.ExpressApp.Web;

namespace EFDemo.Web {
	public class EFDemoWebApplication : XpandWebApplication {
		private DevExpress.ExpressApp.SystemModule.SystemModule systemModule1;
		private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule webSystemModule1;
		private SecurityModule securityModule1;
		private SecurityStrategyComplex securityStrategyComplex1;
		private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsWebModule1;
		private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
		private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
		private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
		private EFDemo.Module.Web.EFDemoWebModule efDemoWebModule1;
		private EFDemo.Module.EFDemoModule efDemoModule1;
		private AuthenticationStandard authenticationStandard1;
		private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
		private DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule schedulerAspNetModule1;
		private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
		private DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule reportsWebModule1;
		private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
		private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
		private DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule htmlPropertyEditorAspNetModule1;
		private System.Data.SqlClient.SqlConnection sqlConnection1;
		private DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule scriptRecorderAspNetModule1;
		private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EFDemoWebApplication));
			this.systemModule1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
			this.webSystemModule1 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
			this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
			this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
			this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
			this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
			this.fileAttachmentsWebModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
			this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
			this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
			this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
			this.efDemoModule1 = new EFDemo.Module.EFDemoModule();
			this.efDemoWebModule1 = new EFDemo.Module.Web.EFDemoWebModule();
			this.schedulerAspNetModule1 = new DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule();
			this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
			this.reportsWebModule1 = new DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule();
			this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
			this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
			this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
			this.htmlPropertyEditorAspNetModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule();
			this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
			this.scriptRecorderAspNetModule1 = new DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// sqlConnection1
			// 
			this.sqlConnection1.ConnectionString = @"Integrated Security=True;Pooling=false;MultipleActiveResultSets=True;Data Source=.\SQLExpress;Initial Catalog=EFDemo_v12.2";
			this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
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
			// reportsModule1
			// 
			this.reportsModule1.EnableInplaceReports = true;
			this.reportsModule1.ReportDataType = typeof(EFDemo.Module.Data.ReportData_EF);
			this.reportsModule1.ShowAdditionalNavigation = false;
			// 
			// validationModule1
			// 
			this.validationModule1.AllowValidationDetailsAccess = true;
			// 
			// viewVariantsModule1
			// 
			this.viewVariantsModule1.GenerateVariantsNode = false;
			this.viewVariantsModule1.ShowAdditionalNavigation = false;
			// 
			// pivotChartModuleBase1
			// 
			this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
			// 
			// EFDemoWebApplication
			// 
			this.ApplicationName = "EFDemo";
			this.Connection = this.sqlConnection1;
			this.Modules.Add(this.systemModule1);
			this.Modules.Add(this.webSystemModule1);
			this.Modules.Add(this.securityModule1);
			this.Modules.Add(this.fileAttachmentsWebModule1);
			this.Modules.Add(this.validationModule1);
			this.Modules.Add(this.viewVariantsModule1);
			this.Modules.Add(this.conditionalAppearanceModule1);
			this.Modules.Add(this.efDemoModule1);
			this.Modules.Add(this.efDemoWebModule1);
			this.Modules.Add(this.schedulerModuleBase1);
			this.Modules.Add(this.schedulerAspNetModule1);
			this.Modules.Add(this.reportsModule1);
			this.Modules.Add(this.reportsWebModule1);
			this.Modules.Add(this.pivotChartModuleBase1);
			this.Modules.Add(this.pivotChartAspNetModule1);
			this.Modules.Add(this.htmlPropertyEditorAspNetModule1);
			this.Modules.Add(this.scriptRecorderModuleBase1);
			this.Modules.Add(this.scriptRecorderAspNetModule1);
			this.Security = this.securityStrategyComplex1;
			this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.EFDemoWebApplication_DatabaseVersionMismatch);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}
		private void EFDemoWebApplication_DatabaseVersionMismatch(Object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomProvider(args.ConnectionString, (TypesInfo)TypesInfo, args.ObjectSpaceProviders, () => base.OnCreateCustomObjectSpaceProvider(args));
        }

		protected override void OnLoggedOn(LogonEventArgs args) {
			base.OnLoggedOn(args);
			((ShowViewStrategy)base.ShowViewStrategy).CollectionsEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
		}

		public EFDemoWebApplication() {
			InitializeComponent();
			DelayedViewItemsInitialization = true;
		}
	}
}

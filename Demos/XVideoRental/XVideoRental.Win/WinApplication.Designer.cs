

using DevExpress.Persistent.BaseImpl;
using XVideoRental.Module.Win;
#pragma warning disable CS0618 // Type or member is obsolete


namespace XVideoRental.Win {
    partial class XVideoRentalWindowsFormsApplication {
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
            this.sqlConnection1 = new Microsoft.Data.SqlClient.SqlConnection();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.pivotGridModule1 = new DevExpress.ExpressApp.PivotGrid.PivotGridModule();
            this.chartModule1 = new DevExpress.ExpressApp.Chart.ChartModule();
            this.pivotChartWindowsFormsModule1 = new DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule();
            this.pivotGridWindowsFormsModule1 = new DevExpress.ExpressApp.PivotGrid.Win.PivotGridWindowsFormsModule();
            this.chartWindowsFormsModule1 = new DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule();
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerWindowsFormsModule1 = new DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule();
            this.module4 = new XVideoRental.Module.Win.XVideoRentalWindowsFormsModule();
            
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=X" +
    "VideoRental";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(Xpand.ExpressApp.Security.Core.XpandRole);
            this.securityStrategyComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // viewVariantsModule1
            // 

            this.viewVariantsModule1.ShowAdditionalNavigation = false;
            // 
            // pivotChartModuleBase1
            // 
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // reportsModule1
            // 
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            // 
            // XVideoRentalWindowsFormsApplication
            // 
            this.ApplicationName = "XVideoRental";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.pivotGridModule1);
            this.Modules.Add(this.chartModule1);
            this.Modules.Add(this.pivotChartWindowsFormsModule1);
            this.Modules.Add(this.pivotGridWindowsFormsModule1);
            this.Modules.Add(this.chartWindowsFormsModule1);
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerWindowsFormsModule1);
            this.Modules.Add(this.securityModule1);
            
            this.Modules.Add(this.validationModule1);
            
            this.Modules.Add(this.treeListEditorsModuleBase1);
            
            this.Modules.Add(this.module4);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.XVideoRentalWindowsFormsApplication_DatabaseVersionMismatch);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.XVideoRentalWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private XVideoRentalWindowsFormsModule module4;

        private Microsoft.Data.SqlClient.SqlConnection sqlConnection1;

        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityStrategyComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.PivotGrid.PivotGridModule pivotGridModule1;
        private DevExpress.ExpressApp.Chart.ChartModule chartModule1;
        private DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule pivotChartWindowsFormsModule1;
        private DevExpress.ExpressApp.PivotGrid.Win.PivotGridWindowsFormsModule pivotGridWindowsFormsModule1;
        private DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule chartWindowsFormsModule1;
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule schedulerWindowsFormsModule1;
//        private Xpand.ExpressApp.SystemModule.XpandSystemModule xpandSystemModule1;
//        private Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule xpandSystemWindowsFormsModule1;
//        private Xpand.ExpressApp.PivotGrid.Win.XpandPivotGridWinModule xpandPivotGridWinModule1;
//        private Xpand.ExpressApp.Chart.Win.XpandChartWinModule xpandChartWinModule1;
//        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
//        private Xpand.ExpressApp.MasterDetail.MasterDetailModule masterDetailModule1;
//        private Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule masterDetailWindowsModule1;
//        private Xpand.ExpressApp.Scheduler.Win.XpandSchedulerWindowsFormsModule _xpandSchedulerWindowsFormsModule2;
//        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
//        private Xpand.ExpressApp.Security.Win.XpandSecurityWinModule xpandSecurityWinModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
//        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;


//        private Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule modelArtifactStateModule1;
        
//        private Xpand.ExpressApp.Dashboard.DashboardModule dashboardModule1;
//        private Xpand.ExpressApp.XtraDashboard.Win.DashboardWindowsFormsModule dashboardWindowsFormsModule1;
        
    }
}

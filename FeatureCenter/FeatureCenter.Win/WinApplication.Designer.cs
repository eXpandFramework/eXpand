

namespace FeatureCenter.Win
{
    partial class FeatureCenterWindowsFormsApplication
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
            this.conditionalFormattingModule1 = new DevExpress.ExpressApp.ConditionalFormatting.ConditionalFormattingModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            
            this.conditionalEditorStateModuleBase1 = new DevExpress.ExpressApp.ConditionalEditorState.ConditionalEditorStateModuleBase();
            
            this.conditionalEditorStateWindowsFormsModule1 = new DevExpress.ExpressApp.ConditionalEditorState.Win.ConditionalEditorStateWindowsFormsModule();
            this.fileAttachmentsWindowsFormsModule1 = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
            this.htmlPropertyEditorWindowsFormsModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.pivotChartWindowsFormsModule1 = new DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule();
            this.printingWindowsFormsModule1 = new DevExpress.ExpressApp.Printing.Win.PrintingWindowsFormsModule();
            this.reportsWindowsFormsModule1 = new DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule();
            
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.schedulerWindowsFormsModule1 = new DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule();
            this.treeListEditorsModuleBase1 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.treeListEditorsWindowsFormsModule1 = new DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule();
            
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityComplex();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();

            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module1
            // 
            this.module1.AdditionalBusinessClasses.Add(typeof(DevExpress.Xpo.XPObjectType));
            // 
            // module3
            // 
            
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=FeatureCenter;Integrated Security=SSPI;Pooling=false";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // auditTrailModule1
            // 
            
            // 
            // viewVariantsModule1
            // 
            this.viewVariantsModule1.ShowAdditionalNavigation = false;
            // 
            // pivotChartModuleBase1
            // 
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // reportsWindowsFormsModule1
            // 
            
            

            // 
            // securityComplex1
            // 
            this.securityComplex1.Authentication = this.authenticationStandard1;
            this.securityComplex1.IsGrantedForNonExistentPermission = false;
            this.securityComplex1.RoleType = typeof(DevExpress.Persistent.BaseImpl.Role);
            this.securityComplex1.UserType = typeof(DevExpress.Persistent.BaseImpl.User);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // FeatureCenterWindowsFormsApplication
            // 
            this.ApplicationName = "FeatureCenter";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module6);
            
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.conditionalFormattingModule1);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.viewVariantsModule1);
            
            this.Modules.Add(this.conditionalEditorStateModuleBase1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.conditionalEditorStateWindowsFormsModule1);
            this.Modules.Add(this.fileAttachmentsWindowsFormsModule1);
            this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.pivotChartWindowsFormsModule1);
            this.Modules.Add(this.printingWindowsFormsModule1);
            this.Modules.Add(this.reportsWindowsFormsModule1);
            
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerWindowsFormsModule1);
            this.Modules.Add(this.treeListEditorsModuleBase1);
            this.Modules.Add(this.treeListEditorsWindowsFormsModule1);
            this.Modules.Add(this.module7);
            
            this.Modules.Add(this.module4);
            this.Modules.Add(this.securityModule1);
            
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
        private DevExpress.ExpressApp.ConditionalFormatting.ConditionalFormattingModule conditionalFormattingModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        
        private DevExpress.ExpressApp.ConditionalEditorState.ConditionalEditorStateModuleBase conditionalEditorStateModuleBase1;
        
        private DevExpress.ExpressApp.ConditionalEditorState.Win.ConditionalEditorStateWindowsFormsModule conditionalEditorStateWindowsFormsModule1;
        private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule fileAttachmentsWindowsFormsModule1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule pivotChartWindowsFormsModule1;
        private DevExpress.ExpressApp.Printing.Win.PrintingWindowsFormsModule printingWindowsFormsModule1;
        private DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule reportsWindowsFormsModule1;
        
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule schedulerWindowsFormsModule1;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase1;
        private DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule treeListEditorsWindowsFormsModule1;
        
        private DevExpress.ExpressApp.Security.SecurityComplex securityComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
    }
}

namespace WorkflowDemo.Win
{
	partial class WorkflowDemoWindowsFormsApplication
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
			this.module4 = new WorkflowDemo.Module.Win.WorkflowDemoWindowsFormsModule();
			this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
			this.module7 = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
			this.module8 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
			this.htmlPropertyEditorWindowsFormsModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
			this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
			this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
			this.wfModule = new DevExpress.ExpressApp.Workflow.WorkflowModule();
			this.wfWinModule = new DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule();
            
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            this.wfModule.WorkflowDefinitionType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowDefinition);
            this.wfModule.RunningWorkflowInstanceInfoType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoRunningWorkflowInstanceInfo);
            this.wfModule.WorkflowControlCommandRequestType = typeof(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowInstanceControlCommandRequest);
            // 
			// WorkflowDemoWindowsFormsApplication
            // 
			this.ApplicationName = "WorkflowDemo";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.module6);
			this.Modules.Add(this.module7);
			this.Modules.Add(this.module8);
			this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule1);
			this.Modules.Add(this.viewVariantsModule1);
			this.Modules.Add(this.conditionalAppearanceModule1);
			this.Modules.Add(this.wfModule);
			this.Modules.Add(this.wfWinModule);
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.WorkflowDemoWindowsFormsApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
		private WorkflowDemo.Module.Win.WorkflowDemoWindowsFormsModule module4;
		private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
		private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule module7;
		private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule module8;
		private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule1;
		private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
		private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
		private DevExpress.ExpressApp.Workflow.WorkflowModule wfModule;
		private DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule wfWinModule;
	}
}

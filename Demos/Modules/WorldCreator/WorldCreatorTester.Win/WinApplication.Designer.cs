using DevExpress.ExpressApp;

namespace WorldCreatorTester.Win {
    partial class WorldCreatorTesterWindowsFormsApplication {
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
            this.module3 = new WorldCreatorTester.Module.WorldCreatorTesterModule();
            this.module4 = new WorldCreatorTester.Module.Win.WorldCreatorTesterWindowsFormsModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.xpandValidationModule1 = new Xpand.ExpressApp.Validation.XpandValidationModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.xpandSecurityModule1 = new Xpand.ExpressApp.Security.XpandSecurityModule();
            this.worldCreatorModule1 = new Xpand.ExpressApp.WorldCreator.WorldCreatorModule();
            this.worldCreatorDBMapperModule1 = new Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule();
            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.validationWindowsFormsModule1 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
            this.xpandSecurityWinModule1 = new Xpand.ExpressApp.Security.Win.XpandSecurityWinModule();
            this.fileAttachmentsWindowsFormsModule1 = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
            this.worldCreatorWinModule1 = new Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule();
            this.scriptRecorderWindowsFormsModule1 = new DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=W" +
    "orldCreatorTester";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            this.validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // WorldCreatorTesterWindowsFormsApplication
            // 
            this.ApplicationName = "WorldCreatorTester";
            this.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.validationModule1);
            this.Modules.Add(this.xpandValidationModule1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.xpandSecurityModule1);
            this.Modules.Add(this.worldCreatorModule1);
            this.Modules.Add(this.worldCreatorDBMapperModule1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.validationWindowsFormsModule1);
            this.Modules.Add(this.xpandSecurityWinModule1);
            this.Modules.Add(this.fileAttachmentsWindowsFormsModule1);
            this.Modules.Add(this.worldCreatorWinModule1);
            this.Modules.Add(this.scriptRecorderWindowsFormsModule1);
            this.Modules.Add(this.module4);
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.WorldCreatorTesterWindowsFormsApplication_DatabaseVersionMismatch);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.WorldCreatorTesterWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private WorldCreatorTester.Module.WorldCreatorTesterModule module3;
        private WorldCreatorTester.Module.Win.WorldCreatorTesterWindowsFormsModule module4;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
        private Xpand.ExpressApp.Validation.XpandValidationModule xpandValidationModule1;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private Xpand.ExpressApp.Security.XpandSecurityModule xpandSecurityModule1;
        private Xpand.ExpressApp.WorldCreator.WorldCreatorModule worldCreatorModule1;
        private Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule worldCreatorDBMapperModule1;
        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule validationWindowsFormsModule1;
        private Xpand.ExpressApp.Security.Win.XpandSecurityWinModule xpandSecurityWinModule1;
        private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule fileAttachmentsWindowsFormsModule1;
        private Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule worldCreatorWinModule1;
        private DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule scriptRecorderWindowsFormsModule1;
    }
}

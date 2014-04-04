using Xpand.ExpressApp.Security.Core;

namespace AuditTrailTester.Win {
    partial class AuditTrailTesterWindowsFormsApplication {
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
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.auditTrailModule1 = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            this.logicModule1 = new Xpand.ExpressApp.Logic.LogicModule();
            this.xpandAuditTrailModule1 = new Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this.module3 = new AuditTrailTester.Module.AuditTrailTesterModule();
            this.module4 = new AuditTrailTester.Module.Win.AuditTrailTesterWindowsFormsModule();
            this.logicModule2 = new Xpand.ExpressApp.Logic.LogicModule();
            this.xpandAuditTrailModule2 = new Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=A" +
    "uditTrailTester";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // auditTrailModule1
            // 
            this.auditTrailModule1.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(XpandRole);
            this.securityStrategyComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // AuditTrailTesterWindowsFormsApplication
            // 
            this.ApplicationName = "AuditTrailTester";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.auditTrailModule1);
            this.Modules.Add(this.logicModule1);
            this.Modules.Add(this.xpandAuditTrailModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.module4);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.AuditTrailTesterWindowsFormsApplication_DatabaseVersionMismatch);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.AuditTrailTesterWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private AuditTrailTester.Module.AuditTrailTesterModule module3;
        private AuditTrailTester.Module.Win.AuditTrailTesterWindowsFormsModule module4;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule1;
        private Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule xpandAuditTrailModule1;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityStrategyComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
        private Xpand.ExpressApp.Logic.LogicModule logicModule2;
        private Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule xpandAuditTrailModule2;
    }
}

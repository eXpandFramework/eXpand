using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using EmailTester.Module;
using EmailTester.Module.BusinessObjects;
using EmailTester.Module.Win;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;

namespace EmailTester.Win {
    partial class EmailTesterWindowsFormsApplication {
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
            this.module3 = new EmailTester.Module.EmailTesterModule();
            this.module4 = new EmailTester.Module.Win.EmailTesterWindowsFormsModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();

            this._securityStrategyComplex = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this._authenticationStandard = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Integrated Security=SSPI;Pooling=false;Data Source=.\\SQLEXPRESS;Initial Catalog=M" +
    "odelDifferenceTester";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // _securityStrategyComplex
            // 
            this._securityStrategyComplex.Authentication = this._authenticationStandard;
            this._securityStrategyComplex.UserType = typeof(User);
            this._securityStrategyComplex.RoleType = typeof(SecuritySystemRole);
            // 
            // _authenticationStandard
            // 

            this._authenticationStandard.LogonParametersType = typeof(XpandLogonParameters);
            // 
            // EmailTesterWindowsFormsApplication
            // 
            this.ApplicationName = "EmailTester";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.securityModule1);

            this.Modules.Add(this.module4);
            this.Security = this._securityStrategyComplex;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.EmailTesterWindowsFormsApplication_DatabaseVersionMismatch);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.EmailTesterWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private EmailTesterModule module3;
        private EmailTesterWindowsFormsModule module4;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;

        private DevExpress.ExpressApp.Security.SecurityStrategyComplex _securityStrategyComplex;
        private AuthenticationStandard _authenticationStandard;
    }
}

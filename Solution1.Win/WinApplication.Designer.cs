using eXpand.Persistent.TaxonomyImpl;

namespace Solution1.Win
{
    partial class Solution1WindowsFormsApplication
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
            this.module5 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.module7 = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
            this.cloneObjectModule1 = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.modelDifferenceModule1 = new eXpand.ExpressApp.ModelDifference.ModelDifferenceModule();
            this.eXpandSecurityModule1 = new eXpand.ExpressApp.Security.eXpandSecurityModule();
            this.eXpandValidationModule1 = new eXpand.ExpressApp.Validation.eXpandValidationModule();
            this.eXpandSystemModule1 = new eXpand.ExpressApp.SystemModule.eXpandSystemModule();
            this.modelArtifactStateModule1 = new eXpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule();
            this.modelDifferenceWindowsFormsModule1 = new eXpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule();
            this.eXpandSystemWindowsFormsModule1 = new eXpand.ExpressApp.Win.SystemModule.eXpandSystemWindowsFormsModule();
            this.module3 = new Solution1.Module.Solution1Module();
            this.module4 = new Solution1.Module.Win.Solution1WindowsFormsModule();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityComplex();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // module1
            // 
            this.module1.AdditionalBusinessClasses.Add(typeof(DevExpress.Xpo.XPObjectType));
            // 
            // module5
            // 
            this.module5.AllowValidationDetailsAccess = true;
            // 
            // sqlConnection1
            // 
            this.sqlConnection1.ConnectionString = "Data Source=(local);Initial Catalog=Solution1;Integrated Security=SSPI;Pooling=fa" +
                "lse";
            this.sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // securityComplex1
            // 
            this.securityComplex1.Authentication = this.authenticationStandard1;
            this.securityComplex1.IsGrantedForNonExistentPermission = false;
            this.securityComplex1.RoleType = typeof(Role);
            this.securityComplex1.UserType = typeof(User);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // Solution1WindowsFormsApplication
            // 
            this.ApplicationName = "Solution1";
            this.Connection = this.sqlConnection1;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.cloneObjectModule1);
            this.Modules.Add(this.modelDifferenceModule1);
            this.Modules.Add(this.module5);
            this.Modules.Add(this.eXpandSecurityModule1);
            this.Modules.Add(this.eXpandValidationModule1);
            this.Modules.Add(this.eXpandSystemModule1);
            this.Modules.Add(this.modelArtifactStateModule1);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.modelDifferenceWindowsFormsModule1);
            this.Modules.Add(this.eXpandSystemWindowsFormsModule1);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.module7);
            this.Modules.Add(this.securityModule1);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.Solution1WindowsFormsApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;

        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private Solution1.Module.Solution1Module module3;
        private Solution1.Module.Win.Solution1WindowsFormsModule module4;
        private DevExpress.ExpressApp.Validation.ValidationModule module5;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule module7;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private System.Data.SqlClient.SqlConnection sqlConnection1;
        private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule1;
        private eXpand.ExpressApp.ModelDifference.ModelDifferenceModule modelDifferenceModule1;
        private eXpand.ExpressApp.Security.eXpandSecurityModule eXpandSecurityModule1;
        private eXpand.ExpressApp.Validation.eXpandValidationModule eXpandValidationModule1;
        private eXpand.ExpressApp.SystemModule.eXpandSystemModule eXpandSystemModule1;
        private eXpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule modelArtifactStateModule1;
        private eXpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule modelDifferenceWindowsFormsModule1;
        private eXpand.ExpressApp.Win.SystemModule.eXpandSystemWindowsFormsModule eXpandSystemWindowsFormsModule1;
        private DevExpress.ExpressApp.Security.SecurityComplex securityComplex1;
        private DevExpress.ExpressApp.Security.AuthenticationStandard authenticationStandard1;
    }
}

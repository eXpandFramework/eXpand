namespace SecurityDemo.UiLevel.Win
{
	partial class SecurityDemoWindowsFormsApplication
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
			this.module3 = new SecurityDemo.Module.SecurityDemoModule();
			this.module4 = new SecurityDemo.Module.Win.SecurityDemoWindowsFormsModule();
            this.module6 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.module7 = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.module8 = new DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.authentication1 = new SecurityDemo.Module.SecurityDemoAuthentication();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();

            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // securityComplex1
            // 
            this.ConnectionString = @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=SecurityDemo_v12.2";
            this.securityComplex1.Authentication = this.authentication1;
            this.securityComplex1.UserType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemUser);
            this.securityComplex1.RoleType = typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemRole);
            // 
			// SecurityDemoWindowsFormsApplication
            // 
			this.ApplicationName = "SecurityDemo";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.module6);
            this.Modules.Add(this.module7);
            this.Modules.Add(this.module8);
            this.Modules.Add(this.securityModule1);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.SecurityDemoWindowsFormsApplication_DatabaseVersionMismatch);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
		private SecurityDemo.Module.SecurityDemoModule module3;
		private SecurityDemo.Module.Win.SecurityDemoWindowsFormsModule module4;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule module6;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase module7;
        private DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule module8;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private SecurityDemo.Module.SecurityDemoAuthentication authentication1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityComplex1;
    }
}

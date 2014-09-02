using DCSecurityDemo.Module;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Web;

namespace DCSecurityDemo.UiLevel.Web {
    public partial class DCSecurityDemoAspNetApplication : XpandWebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule module2;
        private DCSecurityDemo.Module.DCSecurityDemoModule module3;
        private SecurityDemo.Module.Web.SecurityDemoAspNetModule module4;
        private DevExpress.ExpressApp.Security.SecurityModule securityModule1;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule;
        private DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule module8;
        private DCSecurityDemo.Module.DCSecurityDemoAuthentication authentication1;
        private DevExpress.ExpressApp.Security.SecurityStrategyComplex securityComplex1;

        public DCSecurityDemoAspNetApplication() {
            InitializeComponent();
        }


        private void DCSecurityDemoAspNetApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            try {
                e.Updater.Update();
                e.Handled = true;
            } catch (CompatibilityException exception) {
                if (exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
        private void DCSecurityDemoAspNetApplication_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(CreateController<ShowHintController>());
        }
        private void DCSecurityDemoAspNetApplication_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = CreateObjectSpace();
            ((DCSecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        private void InitializeComponent() {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.module3 = new DCSecurityDemo.Module.DCSecurityDemoModule();
            this.module4 = new SecurityDemo.Module.Web.SecurityDemoAspNetModule();
            this.module8 = new DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule();
            this.validationModule = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.authentication1 = new DCSecurityDemo.Module.DCSecurityDemoAuthentication();
            this.securityComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();

            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // securityComplex1
            // 
            this.ConnectionString = @"Integrated Security=SSPI;Pooling=false;Data Source=.\SQLEXPRESS;Initial Catalog=DCSecurityDemo_v12.2";
            this.securityComplex1.Authentication = this.authentication1;
            this.securityComplex1.UserType = typeof(DCSecurityDemo.Module.Security.IDCUser);
            this.securityComplex1.RoleType = typeof(DCSecurityDemo.Module.Security.IDCRole);
            // 
            // DCSecurityDemoAspNetApplication
            // 
            this.ApplicationName = "DCSecurityDemo";
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.module8);
            this.Modules.Add(this.validationModule);
            this.Modules.Add(this.securityModule1);
            this.Security = this.securityComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.DCSecurityDemoAspNetApplication_DatabaseVersionMismatch);
            this.CreateCustomLogonWindowControllers += new System.EventHandler<DevExpress.ExpressApp.CreateCustomLogonWindowControllersEventArgs>(this.DCSecurityDemoAspNetApplication_CreateCustomLogonWindowControllers);
            this.CreateCustomLogonWindowObjectSpace += new System.EventHandler<DevExpress.ExpressApp.CreateCustomLogonWindowObjectSpaceEventArgs>(this.DCSecurityDemoAspNetApplication_CreateCustomLogonWindowObjectSpace);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }
}

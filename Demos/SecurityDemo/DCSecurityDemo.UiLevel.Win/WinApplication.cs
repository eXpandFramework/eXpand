using DCSecurityDemo.Module;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Win;

namespace DCSecurityDemo.UiLevel.Win {
    public partial class DCSecurityDemoWindowsFormsApplication : XpandWinApplication {
        public DCSecurityDemoWindowsFormsApplication() {
            InitializeComponent();
        }

        private void DCSecurityDemoWindowsFormsApplication_CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e) {
            if(e.Context.Name == TemplateContext.ApplicationWindow) {
                e.Template = new FeatureCenter.Module.Win.MainForm();
            }
            if(e.Context.Name == TemplateContext.PopupWindow) {
                e.Template = new FeatureCenter.Module.Win.PopupForm();
            }
        }
        private void DCSecurityDemoWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            try {
                e.Updater.Update();
                e.Handled = true;
            }
            catch(CompatibilityException exception) {
                if(exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
        private void DCSecurityDemoWindowsFormsApplication_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(CreateController<ShowHintController>());
        }
        private void DCSecurityDemoWindowsFormsApplication_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = CreateObjectSpace();
            ((DCSecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }
    }
}

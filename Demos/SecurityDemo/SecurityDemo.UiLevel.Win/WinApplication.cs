using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Win;
using Xpand.Persistent.Base.General;

namespace SecurityDemo.UiLevel.Win {
    public partial class SecurityDemoWindowsFormsApplication : XpandWinApplication {
        public SecurityDemoWindowsFormsApplication() {
            InitializeComponent();
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args, null);
        }

        private void SecurityDemoWindowsFormsApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            try {
#if EASYTEST
            e.Updater.Update();
                e.Handled = true;
#else
                if (Debugger.IsAttached){
                    e.Updater.Update();
                    e.Handled = true;
                }
#endif
                
            } catch (CompatibilityException exception) {
                if (exception.Error is CompatibilityUnableToOpenDatabaseError) {
                    throw new UserFriendlyException(
                    "The connection to the database failed. This demo requires the local instance of Microsoft SQL Server Express. To use another database server,\r\nopen the demo solution in Visual Studio and modify connection string in the \"app.config\" file.");
                }
            }
        }
    }
}

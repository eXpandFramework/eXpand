using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Xpo;
using SecuritySystemExample.Module;
using Xpand.ExpressApp.MiddleTier;

namespace ConsoleApplicationServer {
    public class ConsoleApplicationServerServerApplication : XpandServerApplication {
        public ConsoleApplicationServerServerApplication(ISecurityStrategyBase securityStrategyBase) : base(securityStrategyBase) {
            ApplicationName = "SecuritySystemExample";
            
            Modules.Add(new SystemWindowsFormsModule());
            Modules.Add(new SecuritySystemExampleModule());
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
    }
}
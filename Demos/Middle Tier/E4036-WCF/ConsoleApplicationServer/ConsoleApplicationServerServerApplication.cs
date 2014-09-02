using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using SecuritySystemExample.Module;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.MiddleTier;

namespace ConsoleApplicationServer {
    public class ConsoleApplicationServerServerApplication : XpandServerApplication {
        public ConsoleApplicationServerServerApplication(ISecurityStrategyBase securityStrategyBase)
            : base(securityStrategyBase) {
            ApplicationName = "SecuritySystemExample";
            Modules.Add(new SecuritySystemExampleModule());
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args,null);
        }

    }
}
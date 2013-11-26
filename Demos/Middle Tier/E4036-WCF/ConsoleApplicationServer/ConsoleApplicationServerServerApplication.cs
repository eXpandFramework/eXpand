using DevExpress.ExpressApp.Security;
using SecuritySystemExample.Module;
using Xpand.Persistent.Base.MiddleTier;

namespace ConsoleApplicationServer {
    public class ConsoleApplicationServerServerApplication : XpandServerApplication {
        public ConsoleApplicationServerServerApplication(ISecurityStrategyBase securityStrategyBase,bool wfc)
            : base(securityStrategyBase,wfc) {
            ApplicationName = "SecuritySystemExample";
            Modules.Add(new SecuritySystemExampleModule());
        }
    }
}
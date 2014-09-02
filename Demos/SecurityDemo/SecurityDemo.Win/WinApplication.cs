using System.Linq;
using DevExpress.ExpressApp.Utils;
using SecurityDemo.Module;
using Xpand.ExpressApp.Win;

namespace SecurityDemo.Win {
    public partial class SecurityDemoWindowsFormsApplication : XpandWinApplication {
        public SecurityDemoWindowsFormsApplication() {
            InitializeComponent();
        }

//        protected override void ReadLastLogonParametersCore(SettingsStorage storage, object logonObject){
//            base.ReadLastLogonParametersCore(storage, logonObject);
//            var logonParameters = ((SecurityDemoAuthenticationLogonParameters) logonObject);
//            logonParameters.User = logonParameters.AvailableUsers.Last();
//        }
    }
}

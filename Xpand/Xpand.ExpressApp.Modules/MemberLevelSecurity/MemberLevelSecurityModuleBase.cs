using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;

namespace Xpand.ExpressApp.MemberLevelSecurity {
    public abstract class MemberLevelSecurityModuleBase : XpandModuleBase {
        static bool _comparerIsLock;
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (!DesignMode && !_comparerIsLock && typeof(IUser).IsAssignableFrom(Application.Security.UserType)) {
                ObjectAccessComparerBase.SetCurrentComparer(new MemberLevelObjectAccessComparer());
                _comparerIsLock = true;
            }
        }
    }
}
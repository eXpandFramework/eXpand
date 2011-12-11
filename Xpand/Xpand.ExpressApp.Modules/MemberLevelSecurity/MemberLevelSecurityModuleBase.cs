using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Core;

namespace Xpand.ExpressApp.MemberLevelSecurity {
    public abstract class MemberLevelSecurityModuleBase : XpandModuleBase {
        static bool _comparerIsLock;
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (!DesignMode && !_comparerIsLock && SecuritySystem.Instance is ISecurityComplex &&
                !((ISecurityComplex)SecuritySystem.Instance).IsNewSecuritySystem()) {
                ObjectAccessComparerBase.SetCurrentComparer(new MemberLevelObjectAccessComparer());
                _comparerIsLock = true;
            }
        }
    }
}
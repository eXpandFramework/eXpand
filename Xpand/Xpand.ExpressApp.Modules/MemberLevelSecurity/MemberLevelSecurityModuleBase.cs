using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp;

namespace Xpand.ExpressApp.MemberLevelSecurity {
    public abstract class MemberLevelSecurityModuleBase:XpandModuleBase
    {
        static bool _comparerIsLock;
//        protected abstract bool? ComparerIsSet { get; set; }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if (!DesignMode && !_comparerIsLock){
                ObjectAccessComparerBase.SetCurrentComparer(new MemberLevelObjectAccessComparer());
                _comparerIsLock = true;
            }
        }
    }
}
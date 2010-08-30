using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public abstract class MemberLevelSecurityModuleBase:ModuleBase
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
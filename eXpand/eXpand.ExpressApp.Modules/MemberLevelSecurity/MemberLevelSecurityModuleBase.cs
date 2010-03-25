using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public abstract class MemberLevelSecurityModuleBase:ModuleBase
    {
        public abstract bool? ComparerIsSet { get; set; }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if ((bool) (!ComparerIsSet)) {
                ObjectAccessComparerBase.SetCurrentComparer(new MemberLevelObjectAccessComparer());
                ComparerIsSet = true;
            }
        }
    }
}
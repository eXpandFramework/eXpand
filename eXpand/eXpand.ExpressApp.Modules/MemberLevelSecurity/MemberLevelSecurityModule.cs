using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public sealed partial class MemberLevelSecurityModule : ModuleBase {
        public MemberLevelSecurityModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            ObjectAccessComparerBase.SetCurrentComparer(new MemberLevelObjectAccessComparer());
        }
    }
}
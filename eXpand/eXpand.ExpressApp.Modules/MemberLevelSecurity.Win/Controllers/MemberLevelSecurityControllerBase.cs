using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.MemberLevelSecurity.Win.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Controllers {
    public class MemberLevelSecurityControllerBase : ViewController {
        protected bool HasProtectPermission() {
            if (!(SecuritySystem.Instance is ISecurityComplex))
                return ((ISimpleUser) SecuritySystem.CurrentUser).IsAdministrator;
            return SecuritySystem.IsGranted(new MemberLevelSecurityPermission(MemberLevelSecurityPermissionModifier.Allow));
        }
    }
}
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.Security.Core
{
    public class SecuritySystemExtensions
    {
        public static bool IsGranted(IPermission permission, bool isGrantedForNonExistent)
        {
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistent;
            bool granted = SecuritySystem.IsGranted(permission);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return granted;
        }
    }
}

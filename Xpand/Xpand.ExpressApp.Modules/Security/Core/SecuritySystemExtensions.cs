using System.Security;
using System.Security.Permissions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Security.Core {
    public static class SecuritySystemExtensions {
        public static bool IsGranted(IPermission permission, bool isGrantedForNonExistent) {
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistent;
            bool granted = SecuritySystem.IsGranted(permission);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return granted;
        }
        public static bool IsGranted(this IRole role,IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission!=null && permission.IsSubsetOf(getPermission);
        }
    }
}

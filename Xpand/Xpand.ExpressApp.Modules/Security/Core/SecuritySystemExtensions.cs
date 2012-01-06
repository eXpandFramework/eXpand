using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Security.Core {
    public static class SecuritySystemExtensions {
        public static bool IsGranted(IPermission permission, bool isGrantedForNonExistent) {
            var securityComplex = (SecuritySystem.Instance as SecurityBase);
            if (securityComplex != null) {
                bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistent;
                bool granted = SecuritySystem.IsGranted(permission);
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
                return granted;
            }
            return SecuritySystem.IsGranted(permission);
        }
        public static List<IOperationPermission> GetPermissions(this ISecurityUserWithRoles securityUserWithRoles) {
            var permissions = new List<IOperationPermission>();
            foreach (ISecurityRole securityRole in securityUserWithRoles.Roles) {
                IList<IOperationPermission> operationPermissions = securityRole.GetPermissions();
                permissions.AddRange(operationPermissions);
            }
            return permissions;
        }

        public static bool IsNewSecuritySystem(this ISecurityComplex security) {
            return typeof(ISecurityRole).IsAssignableFrom(((ISecurityComplex)SecuritySystem.Instance).RoleType);
        }

        public static bool IsGranted(this IRole role, IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission != null && permission.IsSubsetOf(getPermission);
        }
    }
}

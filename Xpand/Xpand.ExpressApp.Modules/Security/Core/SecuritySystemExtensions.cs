using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Security.Strategy.PermissionMatrix;
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
            var securityComplex = ((ISecurityComplex)SecuritySystem.Instance);
            var permissions = new List<IOperationPermission>();
            foreach (ISecurityRole securityRole in securityUserWithRoles.Roles) {
                if (securityComplex.IsNewSecuritySystem()) {
                    IList<IOperationPermission> operationPermissions = ((SecuritySystemTypePermissionsObjectOwner)securityRole).GetPermissions();
                    permissions.AddRange(operationPermissions);
                } else {
                    var operationPermissions = ((IOperationPermissionsProvider)securityRole).GetPermissions();
                    permissions.AddRange(operationPermissions);
                }
            }
            return permissions;
        }

        public static bool IsNewSecuritySystem(this ISecurityComplex security) {
            return typeof(IPermissionMatrixTypePermissionsOwner).IsAssignableFrom(security.RoleType);
        }

        public static bool IsGranted(this IRole role, IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission != null && permission.IsSubsetOf(getPermission);
        }
    }
}

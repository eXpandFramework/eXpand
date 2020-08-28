using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.Security {
    public static class SecurityExtensions {
        public static IPermissionPolicyUser GetAnonymousPermissionPolicyUser(this IPermissionPolicyRole systemRole) {
            var optionsAthentication = ((IModelOptionsAuthentication)ApplicationHelper.Instance.Application.Model.Options).Athentication;
            var anonymousUserName = optionsAthentication.AnonymousAuthentication.AnonymousUser;
            return (IPermissionPolicyUser) systemRole.GetPermissionPolicyUser(anonymousUserName);
        }
        public static ISecurityUserWithRoles GetPermissionPolicyUser(this IPermissionPolicyRole systemRole, string userName, string passWord = "") {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(systemRole);
            return objectSpace.GetUser(userName, passWord, (ISecurityRole) systemRole);
        }

        public static XpandPermissionPolicyRole GetAnonymousPermissionPolicyRole(this IObjectSpace objectSpace, string roleName, bool selfReadOnlyPermissions = true){
            var anonymousRole = objectSpace.GetRole(roleName) as XpandPermissionPolicyRole;
            anonymousRole?.Permissions.AddRange(new[]{
                objectSpace.CreateModifierPermissionPolicy<MyDetailsOperationPermissionPolicyData>(Modifier.Allow),
                objectSpace.CreateModifierPermissionPolicy<AnonymousLoginOperationPermissionPolicyData>(Modifier.Allow)
            });
            return anonymousRole;
        }

        public static PermissionPolicyData.PermissionPolicyData CreateModifierPermissionPolicy<T>(this IObjectSpace objectSpace, Modifier modifier) where T : ModifierPermissionPolicyData {
            var operationPermissionData = objectSpace.CreateObject<T>();
            operationPermissionData.Modifier = modifier;
            return operationPermissionData;
        }


    }
}

using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.Security.Core;

namespace Xpand.ExpressApp.ModelDifference.Security {
    public static class Extensions {
        public static readonly List<Type> Types = new List<Type> {
                                        typeof (ModelDifferenceObject), typeof (UserModelDifferenceObject),
                                          typeof (RoleModelDifferenceObject), typeof (IntermediateObject), typeof (AspectObject),
                                          typeof (PersistentApplication)
                                      };

        public static ISecurityRole GetDefaultModelRole(this IObjectSpace objectSpace, string roleName) {
            var modelRole = objectSpace.GetRole(roleName);
            if (objectSpace.IsNewObject(modelRole)) {
                var securitySystemRole = modelRole as SecuritySystemRole;
                if (securitySystemRole != null){
                    securitySystemRole.SetTypePermissions<PersistentApplication>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                    securitySystemRole.SetTypePermissions<ModelDifferenceObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                    securitySystemRole.SetTypePermissions<AspectObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                    securitySystemRole.SetTypePermissions<UserModelDifferenceObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                }
                else{
                    var permissionPolicyRole = ((IPermissionPolicyRole) modelRole);
                    permissionPolicyRole.AddTypePermission<PersistentApplication>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                    permissionPolicyRole.AddTypePermission<ModelDifferenceObject>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                    permissionPolicyRole.AddTypePermission<AspectObject>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                    permissionPolicyRole.AddTypePermission<UserModelDifferenceObject>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                }
            }
            return modelRole;
        }

    }
}

using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Security {
    public static class Extensions {
        public static void GrantPermissionsObjects(this ISecurityRole securityRole, List<Type> types) {
            if (SecuritySystem.Instance is ISecurityComplex)
                types.Add(((ISecurityComplex)SecuritySystem.Instance).RoleType);
            foreach (var type in types) {
                TypeOperationPermissionDescriptor descriptor =
                    ((TypePermissionDescriptorsList)((XPBaseObject)securityRole).GetMemberValue("Permissions"))[type];
                if (descriptor != null)
                    descriptor.Grant(SecurityOperations.Write);
            }
        }

    }
}

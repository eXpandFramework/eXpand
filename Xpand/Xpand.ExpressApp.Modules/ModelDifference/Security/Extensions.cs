using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Security {
    public static class Extensions {
        public static void GrantPermissionsForModelDifferenceObjects(this ISecurityRole securityRole) {
            var types = new List<Type> {
                                typeof (ModelDifferenceObject), typeof (UserModelDifferenceObject),
                                  typeof (RoleModelDifferenceObject), typeof (IntermediateObject), typeof (AspectObject),
                                  typeof (PersistentApplication), typeof (XPObjectType),SecuritySystem.UserType
                              };
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

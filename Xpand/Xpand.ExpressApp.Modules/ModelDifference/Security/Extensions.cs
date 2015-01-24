using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
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

        public static SecuritySystemRoleBase GetDefaultModelRole(this IObjectSpace objectSpace, string roleName) {
            var modelRole = objectSpace.GetRole(roleName);
            if (objectSpace.IsNewObject(modelRole)) {
                modelRole.SetTypePermissions<PersistentApplication>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                modelRole.SetTypePermissions<ModelDifferenceObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                modelRole.SetTypePermissions<AspectObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
                modelRole.SetTypePermissions<UserModelDifferenceObject>(SecurityOperations.CRUDAccess,SecuritySystemModifier.Allow );
            }
            return modelRole;
        }

    }
}

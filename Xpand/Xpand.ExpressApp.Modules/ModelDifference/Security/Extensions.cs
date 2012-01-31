using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.Security;

namespace Xpand.ExpressApp.ModelDifference.Security {
    public static class Extensions {
        public static void GrantPermissionsForModelDifferenceObjects(this ISecurityRole securityRole) {
            var types = new List<Type> {
                                typeof (ModelDifferenceObject), typeof (UserModelDifferenceObject),
                                  typeof (RoleModelDifferenceObject), typeof (IntermediateObject), typeof (AspectObject),
                                  typeof (PersistentApplication), typeof (XPObjectType),SecuritySystem.UserType
                              };
            securityRole.GrantPermissionsObjects(types);
        }
    }
}

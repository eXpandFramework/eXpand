using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Security {
    public static class Extensions {
        public static void GrantPermissionsForModelDifferenceObjects(this SecurityRole securityRole) {
            var types = new[] {
                                  typeof (ModelDifferenceObject), typeof (UserModelDifferenceObject),
                                  typeof (RoleModelDifferenceObject), typeof (IntermediateObject), typeof (AspectObject),
                                  typeof (PersistentApplication), typeof (XPObjectType)
                              };
            foreach (var type in types) {
                var descriptor = securityRole.Permissions[type];
                if (descriptor!=null)
                    descriptor.Grant(SecurityOperations.Write);
            }
        }
    }
}

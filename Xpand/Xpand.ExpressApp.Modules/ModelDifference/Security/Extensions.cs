using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
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
                modelRole.CreateTypePermission<PersistentApplication>(o => {
                    o.AllowDelete = false;
                    o.AllowNavigate = false;
                });
                modelRole.CreateTypePermission<ModelDifferenceObject>(o => {
                    o.AllowCreate = false;
                    o.AllowNavigate = false;
                });
                modelRole.CreateTypePermission<UserModelDifferenceObject>(o => {
                    o.AllowDelete = false;
                    o.AllowNavigate = false;
                });
                modelRole.CreateTypePermission<AspectObject>(o => {
                    o.AllowDelete = false;
                    o.AllowNavigate = false;
                    o.AllowCreate = false;
                });
            }
            return modelRole;
        }

    }
}

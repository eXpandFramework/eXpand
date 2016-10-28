using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using ModelDifferenceTester.Module.FunctionalTests.ApplicationModel;
using ModelDifferenceTester.Module.FunctionalTests.RoleModel;
using ModelDifferenceTester.Module.FunctionalTests.UserModel;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security.Core;

namespace ModelDifferenceTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (PermissionPolicyRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = (PermissionPolicyRole) ObjectSpace.GetRole("User");
            var user = (PermissionPolicyUser)userRole.GetUser("User");
            user.Roles.Add(defaultRole);
            user = (PermissionPolicyUser)userRole.GetUser("user2");
            user.Roles.Add(defaultRole);
            userRole.AddTypePermission<RoleModelObject>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);
            userRole.AddTypePermission<UserModelObject>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);
            userRole.AddTypePermission<ApplicationModelObject>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);


            var modelRole = (PermissionPolicyRole)ObjectSpace.GetDefaultModelRole("ModelRole");
            user.Roles.Add(modelRole);

            ObjectSpace.CommitChanges();
        }
    }
}

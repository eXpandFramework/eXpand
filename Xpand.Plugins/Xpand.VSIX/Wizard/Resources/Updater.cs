using System;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Core;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace $projectsuffix$.Module.DatabaseUpdate {

    public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion)
        : base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        var defaultRole = (PermissionPolicyRole)ObjectSpace.GetDefaultRole();

        var adminRole = ObjectSpace.GetAdminRole("Admin");
        adminRole.GetUser("Admin");

        var userRole = ObjectSpace.GetRole("User");
        var user = (PermissionPolicyUser)userRole.GetUser("user");
        user.Roles.Add(defaultRole);

        ObjectSpace.CommitChanges();

    }
}
}

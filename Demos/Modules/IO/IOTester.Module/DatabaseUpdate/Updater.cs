using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;

namespace IOTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<SecuritySystemRole>(null) == null) {
                var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

                var adminRole = ObjectSpace.GetAdminRole("Admin");
                adminRole.GetUser("Admin");

                var userRole = ObjectSpace.GetRole("User");
                SecuritySystemUser user = (SecuritySystemUser)userRole.GetUser("user");
                user.Roles.Add(defaultRole);
            }

        }
    }
}

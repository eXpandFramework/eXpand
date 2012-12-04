using System;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.ModelDifference.Security;

namespace $projectsuffix$.Module {
    public enum PermissionBehaviour {
        None,
        Model
    }

    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            SecuritySystemRole defaultRole = ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = ObjectSpace.GetRole("User");
            var user = userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            var modelRole = ObjectSpace.GetDefaultModelRole("ModelRole");
            user.Roles.Add(modelRole);

            ObjectSpace.CommitChanges();

        }
    }
}

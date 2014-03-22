using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;
using XtraDashboardTester.Module.BusinessObjects;

namespace XtraDashboardTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = ObjectSpace.GetRole("User");
            userRole.CanEditModel = true;
            userRole.SetTypePermissionsRecursively<object>(SecurityOperations.FullAccess, SecuritySystemModifier.Allow);

            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Apostolis";
                customer.LastName = "Bekiaris";



                ObjectSpace.CommitChanges();

            }

            ObjectSpace.CommitChanges();
        }

    }
}

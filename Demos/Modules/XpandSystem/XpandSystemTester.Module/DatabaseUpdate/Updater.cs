using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;
using XpandSystemTester.Module.BusinessObjects;

namespace XpandSystemTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Apostolis";
                customer.LastName = "Bekiaris";
                customer.Email = "apostolis.bekiaris at gmail";
                ObjectSpace.CommitChanges();
                
                customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Tolis";
                customer.LastName = "Bek";
                customer.Email = "apostolis.bekiaris at gmail";

                var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

                var adminRole = ObjectSpace.GetAdminRole("Admin");
                adminRole.GetUser("Admin");

                var userRole = ObjectSpace.GetRole("User");
                var user = (SecuritySystemUser)userRole.GetUser("user");
                user.Roles.Add(defaultRole);
            }

            
        }
    }
}

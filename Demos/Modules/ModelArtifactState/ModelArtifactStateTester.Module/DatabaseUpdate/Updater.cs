using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using ModelArtifactStateTester.Module.BusinessObjects;
using Xpand.ExpressApp.Security.Core;

namespace ModelArtifactStateTester.Module.DatabaseUpdate {
    // Allows you to handle a database update when the application version changes (http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppUpdatingModuleUpdatertopic help article for more details).
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        // Override to specify the database update code which should be performed after the database schema is updated (http://documentation.devexpress.com/#Xaf/DevExpressExpressAppUpdatingModuleUpdater_UpdateDatabaseAfterUpdateSchematopic).
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Country>(null) == null) {
                var country = ObjectSpace.CreateObject<Country>();
                country.Name = "USA";
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.Name = "DevExpress";
                customer.Country = country;
                customer = ObjectSpace.CreateObject<Customer>();
                customer.Name = "Ray";
                customer.Country = country;

                country = ObjectSpace.CreateObject<Country>();
                country.Name = "Netherlands";
                country.EUMember = true;
                customer = ObjectSpace.CreateObject<Customer>();
                customer.Name = "Impactive";
                customer.Country = country;
                customer = ObjectSpace.CreateObject<Customer>();
                customer.Name = "Tolis";
                customer.Country = country;

                var xpandRole = ObjectSpace.CreateObject<XpandRole>();
                xpandRole.Name = "Admin";
                xpandRole.IsAdministrative = true;
                var user = ObjectSpace.CreateObject<SecuritySystemUser>();
                user.UserName = "Admin";
                user.Roles.Add(xpandRole);
                
                xpandRole = ObjectSpace.CreateObject<XpandRole>();
                xpandRole.Name = "User";
                xpandRole.SetTypePermissions(typeof(Customer),SecurityOperations.FullAccess,SecuritySystemModifier.Allow);
                xpandRole.SetTypePermissions(typeof(Country),SecurityOperations.FullAccess,SecuritySystemModifier.Allow);
                user = ObjectSpace.CreateObject<SecuritySystemUser>();
                user.UserName = "User";
                user.Roles.Add(xpandRole);

                ObjectSpace.CommitChanges();
            }
        }

    }
}

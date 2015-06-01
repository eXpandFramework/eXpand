using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using ModelArtifactStateTester.Module.BusinessObjects;
using ModelArtifactStateTester.Module.FunctionalTests.Actions;
using ModelArtifactStateTester.Module.FunctionalTests.Controllers;
using Xpand.ExpressApp.Security.Core;

namespace ModelArtifactStateTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        
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

                var securitySystemRole = (SecuritySystemRole)ObjectSpace.GetRole("Admin");
                securitySystemRole.IsAdministrative = true;
                var user = (SecuritySystemUser)ObjectSpace.GetUser("Admin");
                user.Roles.Add(securitySystemRole);

                securitySystemRole = (SecuritySystemRole) ObjectSpace.GetRole("User");
                var types = new[]{typeof (Customer), typeof (Country), typeof (ActionsObject), typeof (ControllersObject)};
                foreach (var type in types){
                    securitySystemRole.SetTypePermissions(type, SecurityOperations.FullAccess, SecuritySystemModifier.Allow);    
                }                
                user = ObjectSpace.CreateObject<SecuritySystemUser>();
                user.Roles.Add(securitySystemRole);
                user = (SecuritySystemUser) ObjectSpace.GetUser("User");
                user.Roles.Add(securitySystemRole);

                ObjectSpace.CommitChanges();
            }
        }

    }
}

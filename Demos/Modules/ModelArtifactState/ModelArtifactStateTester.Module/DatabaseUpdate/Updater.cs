using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
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
            if (ObjectSpace.FindObject<SecuritySystemUser>(null) == null) {


                var securitySystemRole = (SecuritySystemRole)ObjectSpace.GetRole("Admin");
                securitySystemRole.IsAdministrative = true;
                var user = (SecuritySystemUser)ObjectSpace.GetUser("Admin");
                user.Roles.Add(securitySystemRole);

                securitySystemRole = (SecuritySystemRole) ObjectSpace.GetRole("User");
                var types = new[]{typeof (ActionsObject), typeof (ControllersObject)};
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

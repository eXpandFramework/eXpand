using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using ModelArtifactStateTester.Module.FunctionalTests.Actions;
using ModelArtifactStateTester.Module.FunctionalTests.Controllers;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.Security;

namespace ModelArtifactStateTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<XpandPermissionPolicyRole>(null) == null) {


                var securitySystemRole = (XpandPermissionPolicyRole)ObjectSpace.GetRole("Admin");
                securitySystemRole.IsAdministrative = true;
                var user = (XpandPermissionPolicyUser)ObjectSpace.GetUser("Admin");
                user.Roles.Add(securitySystemRole);

                securitySystemRole = (XpandPermissionPolicyRole) ObjectSpace.GetRole("User");
                var types = new[]{typeof (ActionsObject), typeof (ControllersObject)};
                foreach (var type in types){
                    securitySystemRole.SetTypePermission(type, SecurityOperations.FullAccess, SecurityPermissionState.Allow);    
                }                
                user = ObjectSpace.CreateObject<XpandPermissionPolicyUser>();
                user.Roles.Add(securitySystemRole);
                user = (XpandPermissionPolicyUser) ObjectSpace.GetUser("User");
                user.Roles.Add(securitySystemRole);

                ObjectSpace.CommitChanges();
            }
        }

    }
}

using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.Security;

namespace SecurityTester.Module.Win.FunctionalTests.OverallCustomizationAllowed {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            

            XpandPermissionPolicyRole userRole = (XpandPermissionPolicyRole) ObjectSpace.GetRole("User");
            userRole.SetTypePermission<OverallCustomizationAllowedObject>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);

        }
    }
}

using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;

namespace SecurityTester.Module.Win.FunctionalTests.OverallCustomizationAllowed {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            

            var userRole = ObjectSpace.GetRole("User");
            userRole.EnsureTypePermissions<OverallCustomizationAllowedObject>(SecurityOperations.FullAccess);

        }
    }
}

using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using SecurityTester.Module.BusinessObjects;
using Xpand.ExpressApp.Security.Core;

namespace SecurityTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            var anonymousRole = ObjectSpace.GetAnonymousRole("Anonymous");
            anonymousRole.GetAnonymousUser();

            //add project specific permissions
            anonymousRole.SetTypePermissions<Customer>(SecurityOperations.ReadOnlyAccess, SecuritySystemModifier.Allow);

        }
    }
}

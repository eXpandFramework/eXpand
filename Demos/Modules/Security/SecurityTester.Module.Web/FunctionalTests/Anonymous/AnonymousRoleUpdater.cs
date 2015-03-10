using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;

namespace SecurityTester.Module.Web.FunctionalTests.Anonymous {
    public class AnonymousRoleUpdater:ModuleUpdater {
        public AnonymousRoleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            var anonymousRole = ObjectSpace.GetAnonymousRole("Anonymous");
            anonymousRole.GetAnonymousUser();
            anonymousRole.SetTypePermissions<AnonymousObject>(SecurityOperations.FullAccess,SecuritySystemModifier.Allow);

        }
    }
}

using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using Xpand.Persistent.BaseImpl.Security;

namespace SecurityTester.Module.Web.FunctionalTests.Anonymous {
    public class AnonymousRoleUpdater:ModuleUpdater {
        public AnonymousRoleUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            var anonymousRole = ObjectSpace.GetAnonymousPermissionPolicyRole("Anonymous");
            anonymousRole.GetAnonymousPermissionPolicyUser();
            anonymousRole.AddTypePermission<AnonymousObject>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);

        }
    }
}

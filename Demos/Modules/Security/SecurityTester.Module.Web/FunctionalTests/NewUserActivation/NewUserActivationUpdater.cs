using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.BaseImpl.Security;

namespace SecurityTester.Module.Web.FunctionalTests.NewUserActivation {
    public class NewUserActivationUpdater:ModuleUpdater {
        public NewUserActivationUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.GetObjectsQuery<XpandPermissionPolicyUser>().FirstOrDefault(user => user.UserName== "NewUserActivation")==null){
                var xpandUser = ObjectSpace.CreateObject<XpandPermissionPolicyUser>();
                xpandUser.UserName = "NewUserActivation";
                ObjectSpace.CommitChanges();
            }
        }
    }
}

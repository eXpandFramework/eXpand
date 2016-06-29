using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Security.Core;

namespace SecurityTester.Module.Web.FunctionalTests.NewUserActivation {
    public class NewUserActivationUpdater:ModuleUpdater {
        public NewUserActivationUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.GetObjectsQuery<XpandUser>().FirstOrDefault(user => user.UserName== "NewUserActivation")==null){
                var xpandUser = ObjectSpace.CreateObject<XpandUser>();
                xpandUser.UserName = "NewUserActivation";
                ObjectSpace.CommitChanges();
            }
        }
    }
}

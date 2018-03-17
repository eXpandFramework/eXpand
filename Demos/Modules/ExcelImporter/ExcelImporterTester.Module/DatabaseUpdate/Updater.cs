using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using ExcelImporterTester.Module.BusinessObjects;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.Security;

namespace ExcelImporterTester.Module.DatabaseUpdate {
    
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<XpandPermissionPolicyRole>(null) == null) {
                var defaultRole = (XpandPermissionPolicyRole)ObjectSpace.GetDefaultRole();

                var adminRole = ObjectSpace.GetAdminRole("Admin");
                adminRole.GetUser("Admin");

                var userRole = ObjectSpace.GetRole("User");
                var user = (XpandPermissionPolicyUser)userRole.GetUser("User");
                user.Roles.Add(defaultRole);
            }

            var genderObject = ObjectSpace.CreateObject<GenderObject>();
            genderObject.Gender = "Male";
            genderObject = ObjectSpace.CreateObject<GenderObject>();
            genderObject.Gender = "FeMale";
            ObjectSpace.CommitChanges();

        }
    }
}

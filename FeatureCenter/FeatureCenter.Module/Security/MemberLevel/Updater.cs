using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.MemberLevelSecurity;
using Xpand.Xpo;

namespace FeatureCenter.Module.Security.MemberLevel {
    public class Updater : Module.Updater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var role = ObjectSpace.Session.FindObject<Role>(o => o.Name == "Administrators");
            MemberAccessPermission memberAccessPermission = role.Permissions.OfType<MemberAccessPermission>().FirstOrDefault();
            if (memberAccessPermission == null) {
                var accessPermission = new MemberAccessPermission(typeof(MLSCustomer), "Name", MemberOperation.Read, ObjectAccessModifier.Deny) { Criteria = "City='Paris'" };
                role.AddPermission(accessPermission);
                accessPermission = new MemberAccessPermission(typeof(MLSCustomer), "Name", MemberOperation.Write, ObjectAccessModifier.Deny) { Criteria = "City='New York'" };
                role.AddPermission(accessPermission);
                ObjectSpace.CommitChanges();
            }
        }
    }
}

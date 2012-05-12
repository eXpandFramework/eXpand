using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.MemberLevelSecurity;
using Xpand.Xpo;

namespace FeatureCenter.Module.Security.MemberLevel {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (!Updater.IsNewSecuritySystem) {
                var role = session.FindObject<Role>(o => o.Name == SecurityStrategy.AdministratorRoleName);
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
}

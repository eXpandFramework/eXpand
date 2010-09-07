using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.MemberLevelSecurity;

using System.Linq;
using Xpand.Xpo;

namespace FeatureCenter.Module.Security.MemberLevel
{
    public class Updater:Module.Updater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            var role = Session.FindObject<Role>(o => o.Name=="Administrators");
            MemberAccessPermission memberAccessPermission = role.Permissions.OfType<MemberAccessPermission>().FirstOrDefault();
            if (memberAccessPermission == null) {
                var accessPermission = new MemberAccessPermission(typeof (MLSCustomer), "Name", MemberOperation.Read,ObjectAccessModifier.Deny){Criteria = "City='Paris'"};
                role.AddPermission(accessPermission);
                role.Save();
                accessPermission = new MemberAccessPermission(typeof (MLSCustomer), "Name", MemberOperation.Write,ObjectAccessModifier.Deny){Criteria = "City='New York'"};
                role.AddPermission(accessPermission);
                role.Save();
            }
        }
    }
}

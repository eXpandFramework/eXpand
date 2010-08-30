using System;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.MemberLevelSecurity;
using eXpand.Xpo;
using System.Linq;

namespace FeatureCenter.Module.Security.MemberLevel
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            return;
            var findObject = Session.FindObject<Role>(role => role.Name=="Administrators");
            MemberAccessPermission memberAccessPermission = findObject.Permissions.OfType<MemberAccessPermission>().FirstOrDefault();
            if (memberAccessPermission == null) {
                var accessPermission = new MemberAccessPermission(typeof (MLSCustomer), "Name", MemberOperation.Read,ObjectAccessModifier.Deny){Criteria = "City='Paris'"};
                findObject.AddPermission(accessPermission);
                accessPermission = new MemberAccessPermission(typeof (MLSCustomer), "Name", MemberOperation.Write,ObjectAccessModifier.Deny){Criteria = "City='New York'"};
                findObject.AddPermission(accessPermission);
            }
        }
    }
}

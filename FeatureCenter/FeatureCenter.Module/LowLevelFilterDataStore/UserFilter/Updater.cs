using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter
{
    public class Updater:Xpand.Persistent.BaseImpl.Updater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            var findObject = Session.FindObject<Role>(role => role.Name==Administrators);
            if (findObject == null) throw new NotImplementedException();
            EnsureUserExists("filterbyuser", "filterbyuser", findObject);
        }
    }
}

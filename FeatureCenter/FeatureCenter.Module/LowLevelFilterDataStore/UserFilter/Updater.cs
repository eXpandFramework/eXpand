using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using Xpand.Xpo;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter
{
    public class Updater : Module.Updater
    {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            var findObject = ObjectSpace.Session.FindObject<Role>(role => role.Name==Administrators);
            if (findObject == null) throw new NotImplementedException();
            EnsureUserExists("filterbyuser", "filterbyuser", findObject);
        }
    }
}

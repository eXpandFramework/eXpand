using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using Xpand.Xpo;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter {
    public class Updater:FCUpdater {
        

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater) : base(objectSpace, currentDBVersion, updater) {
            
        }

        

        public void UpdateDatabaseAfterUpdateSchema(Xpand.Persistent.BaseImpl.Updater updater) {

            var session = ((ObjectSpace)ObjectSpace).Session;
            var findObject = session.FindObject<Role>(role => role.Name == Xpand.Persistent.BaseImpl.Updater.Administrators);
            if (findObject == null) throw new NotImplementedException();
            updater.EnsureUserExists("filterbyuser", "filterbyuser", findObject);
        }
    }
}

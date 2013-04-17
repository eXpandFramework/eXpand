using System;
using DevExpress.ExpressApp;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter {
    public class Updater : FCUpdater {


        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {

        }



        //        public void UpdateDatabaseAfterUpdateSchema(Xpand.Persistent.BaseImpl.Updater updater) {
        //
        //            var session = ((XPObjectSpace)ObjectSpace).Session;
        //            var findObject = session.FindObject<Role>(role => role.Name == SecurityStrategy.AdministratorRoleName);
        //            if (findObject == null) throw new NotImplementedException();
        //            updater.EnsureUserExists("filterbyuser", "filterbyuser", findObject);
        //        }
    }
}

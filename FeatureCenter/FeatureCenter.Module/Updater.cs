using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;

namespace FeatureCenter.Module {
    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        protected override DevExpress.Persistent.BaseImpl.User EnsureUserExists(string userName, string firstName, DevExpress.Persistent.BaseImpl.Role role) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, role);
            if (ensureUserExists.UserName == Admin)
                UserFilterProvider.UpdaterUserKey = ensureUserExists.Oid;
            return ensureUserExists;
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();

            new DummyDataBuilder(Session).CreateObjects();
        }

    }
}

using System;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;

namespace FeatureCenter.Module {
    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        protected override IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole customizableRole) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, customizableRole);
            if (ensureUserExists.UserName == Admin)
                UserFilterProvider.UpdaterUserKey = ((User) ensureUserExists).Oid;
            return ensureUserExists;

        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();

            new DummyDataBuilder(Session).CreateObjects();
        }

    }
}

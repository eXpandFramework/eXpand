using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Base;
using Xpand.ExpressApp.FilterDataStore.Providers;

namespace FeatureCenter.Module {
    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) {
        }

        protected override IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole customizableRole) {
            var ensureUserExists = base.EnsureUserExists(userName, firstName, customizableRole);
            if (ensureUserExists.UserName == Admin) {
                ((User) ensureUserExists).SetPassword("Admin");
                ObjectSpace.CommitChanges();
                UserFilterProvider.UpdaterUserKey = ((User) ensureUserExists).Oid;
            }
            return ensureUserExists;

        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();

            new DummyDataBuilder(ObjectSpace).CreateObjects();
        }

    }
}

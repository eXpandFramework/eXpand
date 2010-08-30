using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.LowLevelFilterDataStore.UserFilter
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            User userExists = Module.Updater.EnsureUserExists(Session,"filterbyuser","filterbyuser");
            Role ensureRoleExists = Module.Updater.EnsureRoleExists(Session, "Administrators");
            ensureRoleExists.Users.Add(userExists);
            ensureRoleExists.Save();            
        }
    }
}

using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Logic.Win
{
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
        }
    }
}



using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.IO {
    // Dennis: To use it you should first select the required records and then execute the ShowInAnalysisAction action.

    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
        }
    }
}
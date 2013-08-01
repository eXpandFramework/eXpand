using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            if (CurrentDBVersion == new Version(0, 0, 0, 0)) {
                new TypeSynchronizer().SynchronizeTypes(XpandModuleBase.ConnectionString);
            }
        }
    }
}

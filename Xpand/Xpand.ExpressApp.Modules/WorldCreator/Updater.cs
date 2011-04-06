using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Xpand.ExpressApp.WorldCreator {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            if (CurrentDBVersion == new Version(0, 0, 0, 0)) {
                new TypeSynchronizer().SynchronizeTypes(WorldCreatorModuleBase.FullConnectionString);
            }
        }
    }
}

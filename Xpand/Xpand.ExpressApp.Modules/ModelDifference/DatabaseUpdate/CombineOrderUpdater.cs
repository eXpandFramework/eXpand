using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.DatabaseUpdate {
    class CombineOrderUpdater:ModuleUpdater {
        public CombineOrderUpdater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion){
        }

        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            if (CurrentDBVersion<new Version(18,1,604,0))
                SafeDropIndex();
        }

        private void SafeDropIndex(){
            try {
                DropIndex(nameof(ModelDifferenceObject), "iCombineOrder_ModelDifferenceObject");
            }
            catch {
                // ignored
            }
        }
    }
}

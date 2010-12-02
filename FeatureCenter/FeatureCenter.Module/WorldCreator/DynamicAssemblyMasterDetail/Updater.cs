using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            string name = typeof(WC3LevelMasterDetailModelStore).Name;
            if (new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(name) == null) {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(ObjectSpace.Session).InitializeMembers(name);
                modelDifferenceObject.Name = name;
                ObjectSpace.CommitChanges();
            }
        }
    }
}
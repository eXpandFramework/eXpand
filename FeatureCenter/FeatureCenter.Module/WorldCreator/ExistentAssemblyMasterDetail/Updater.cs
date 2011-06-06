using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {

    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            string name = typeof(ExistentAssemblyMasterDetailModelStore).Name;
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(name, FeatureCenterModule.Application) == null) {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(session).InitializeMembers(name, FeatureCenterModule.Application);
                modelDifferenceObject.Name = name;
                ObjectSpace.CommitChanges();
            }
        }
    }
}
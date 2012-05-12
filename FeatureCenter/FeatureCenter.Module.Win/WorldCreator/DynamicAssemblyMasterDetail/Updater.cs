using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            string name = typeof(WC3LevelMasterDetailModelStore).Name;
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(name, FeatureCenterModule.Application) == null) {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(session).InitializeMembers(name, FeatureCenterModule.Application);
                modelDifferenceObject.Name = name;
                ObjectSpace.CommitChanges();
            }
        }
    }
}
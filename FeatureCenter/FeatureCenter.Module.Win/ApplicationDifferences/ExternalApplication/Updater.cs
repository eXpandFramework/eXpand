using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion,updater) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            var uniqueName = new ExternalApplicationModelStore().Name;
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(uniqueName, "ExternalApplication") == null) {
                var modelDifferenceObject = new ModelDifferenceObject(session).InitializeMembers("ExternalApplication", "ExternalApplication.Win", uniqueName);
                modelDifferenceObject.PersistentApplication.ExecutableName = "ExternalApplication.Win.exe";
                var modelApplicationBuilder = new ModelLoader(modelDifferenceObject.PersistentApplication.ExecutableName);
                var model = modelApplicationBuilder.GetLayer(typeof(ExternalApplicationModelStore), false);
                modelDifferenceObject.CreateAspects(model);
                ObjectSpace.CommitChanges();
            }
        }
    }
}

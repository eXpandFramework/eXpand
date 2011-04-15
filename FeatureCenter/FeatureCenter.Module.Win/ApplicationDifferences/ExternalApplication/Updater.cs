using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            var uniqueName = new ExternalApplicationModelStore().Name;
            if (new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(uniqueName, "ExternalApplication") == null) {
                var modelDifferenceObject = new ModelDifferenceObject(ObjectSpace.Session).InitializeMembers("ExternalApplication", "ExternalApplication.Win", uniqueName);
                modelDifferenceObject.PersistentApplication.ExecutableName = "ExternalApplication.Win.exe";
                var modelApplicationBuilder = new ModelLoader(modelDifferenceObject.PersistentApplication.ExecutableName);
                var model = modelApplicationBuilder.GetLayer(typeof(ExternalApplicationModelStore),false);
                Debug.Print("");
                modelDifferenceObject.CreateAspects(model);
                ObjectSpace.CommitChanges();
            }
        }
    }
}

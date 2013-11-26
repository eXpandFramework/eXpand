using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelDifference;

namespace FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication {
    public class Updater : FCUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            var uniqueName = new ExternalApplicationModelStore().Name;
            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(uniqueName, "ExternalApplication") == null) {
                var modelDifferenceObject = new ModelDifferenceObject(session).InitializeMembers("ExternalApplication", "ExternalApplication.Win", uniqueName);
                modelDifferenceObject.PersistentApplication.ExecutableName = "ExternalApplication.Win.exe";
                var modelApplicationBuilder = new ModelLoader(modelDifferenceObject.PersistentApplication.ExecutableName,XafTypesInfo.Instance);
                InterfaceBuilder.SkipAssemblyCleanup = true;
                var model = modelApplicationBuilder.GetLayer(typeof(ExternalApplicationModelStore), false,info => info.AssignAsInstance());
                InterfaceBuilder.SkipAssemblyCleanup = false;
                modelDifferenceObject.CreateAspects(model);
                ObjectSpace.CommitChanges();
            }
        }
    }
}

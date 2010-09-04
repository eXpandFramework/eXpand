using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.ApplicationDifferences.ExternalApplication
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            var uniqueName = new ExternalApplicationModelStore().Name;
            if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(uniqueName, "ExternalApplication") == null){
                var modelDifferenceObject = new ModelDifferenceObject(Session).InitializeMembers("ExternalApplication", "ExternalApplication", uniqueName,false);
                modelDifferenceObject.PersistentApplication.ExecutableName = "ExternalApplication.Win.exe";
                var modelApplicationBuilder = new ModelApplicationBuilder(modelDifferenceObject.PersistentApplication.ExecutableName);
                var model = modelApplicationBuilder.GetLayer(typeof(ExternalApplicationModelStore));
                modelDifferenceObject.CreateAspects(model);
                modelApplicationBuilder.ResetModel(model);
                modelDifferenceObject.Save();
                
            }
        }
    }
}

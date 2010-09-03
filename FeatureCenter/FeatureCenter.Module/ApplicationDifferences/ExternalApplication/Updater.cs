using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

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
                throw new NotImplementedException();
//                modelDifferenceObject.Model = modelApplicationBuilder.GetLayer(typeof(ExternalApplicationModelStore));
//                modelApplicationBuilder.ResetModel();
                modelDifferenceObject.Save();
                
            }
        }
    }
}

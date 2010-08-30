using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.Win.MasterDetail {
    public class Updater:ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
//            return;
            string modelId = typeof(MasterDetailStore).Name;
            if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(modelId) == null)
            {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(Session).InitializeMembers(modelId);
                modelDifferenceObject.Name = modelId;
                modelDifferenceObject.Save();
            }
        }
    }

}
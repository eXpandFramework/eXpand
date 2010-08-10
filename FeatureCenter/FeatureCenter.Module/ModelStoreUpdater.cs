using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module {
    public  class ModelStoreUpdater:ModuleUpdater {
        public ModelStoreUpdater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            var storeType = GetStoreType();
            if (storeType != null) {
                var name = GetType().Namespace;
                name=name.Substring(name.LastIndexOf(".") + 1);
                if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(name) == null)
                {
                    var modelDifferenceObject = new ModelDifferenceObject(Session).InitializeMembers(name);
                    modelDifferenceObject.Name = name;
                    modelDifferenceObject.Save();
                }
            }
        }

        protected virtual Type GetStoreType() {
            return null;
        }
    }
}
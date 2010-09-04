using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {

    public class Updater:ModuleUpdater {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
//            return;
            string name = typeof(ExistentAssemblyMasterDetailModelStore).Name;
            if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(name) == null) {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(Session).InitializeMembers(name);
                modelDifferenceObject.Name = name;
                modelDifferenceObject.Save();
            }
        }
    }
}
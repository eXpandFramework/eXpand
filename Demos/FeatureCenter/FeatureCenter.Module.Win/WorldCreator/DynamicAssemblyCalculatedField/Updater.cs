using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyCalculatedField {
    public class Updater : FCUpdater {

        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            string name = typeof(WCCalculatedFieldModelStore).Name;

            var session = ((XPObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(name, ApplicationHelper.Instance.Application) == null) {
                ModelDifferenceObject modelDifferenceObject =
                    new ModelDifferenceObject(session).InitializeMembers(name, ApplicationHelper.Instance.Application);
                modelDifferenceObject.Name = name;
                ObjectSpace.CommitChanges();
            }
        }
    }
}
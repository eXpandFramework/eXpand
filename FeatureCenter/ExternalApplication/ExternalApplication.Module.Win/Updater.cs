using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using FeatureCenter.Base;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace ExternalApplication.Module.Win {
    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();
            new DummyDataBuilder(ObjectSpace).CreateObjects();
            if (ObjectSpace.Session.FindObject<PersistentAssemblyInfo>(CriteriaOperator.Parse("Name=?", "TestAssembly")) == null) {
                new PersistentAssemblyInfo(ObjectSpace.Session) { Name = "TestAssembly" };
                ObjectSpace.CommitChanges();
            }
        }
    }
}

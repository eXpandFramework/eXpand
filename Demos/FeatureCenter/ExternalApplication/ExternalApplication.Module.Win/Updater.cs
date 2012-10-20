using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using FeatureCenter.Base;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace ExternalApplication.Module.Win {
    public class Updater : Xpand.Persistent.BaseImpl.Updater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();
            var space = (XPObjectSpace)ObjectSpace;
            new DummyDataBuilder(space).CreateObjects();
            if (space.Session.FindObject<PersistentAssemblyInfo>(CriteriaOperator.Parse("Name=?", "TestAssembly")) == null) {
                new PersistentAssemblyInfo(space.Session) { Name = "TestAssembly" };
                space.CommitChanges();
            }
        }
    }
}

using System;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module
{
    public class Updater : Xpand.Persistent.BaseImpl.Updater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();
            new DummyDataBuilder(Session).CreateObjects();
        }

    }
}

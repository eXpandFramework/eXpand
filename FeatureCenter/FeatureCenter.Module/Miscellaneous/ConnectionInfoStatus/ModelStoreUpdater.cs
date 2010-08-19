using System;
using DevExpress.Xpo;

namespace FeatureCenter.Module.ConnectionInfoStatus
{
    public class ModelStoreUpdater:Module.ModelStoreUpdater
    {
        public ModelStoreUpdater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }

        protected override Type GetStoreType() {
            return typeof(ModelStore);
        }
    }
}

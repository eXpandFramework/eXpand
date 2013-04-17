using System;
using DevExpress.ExpressApp;

namespace FeatureCenter.Module {
    public class FCUpdater {
        readonly IObjectSpace _objectSpace;
        readonly Version _version;

        public FCUpdater(IObjectSpace objectSpace, Version version) {
            _objectSpace = objectSpace;
            _version = version;
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public Version Version {
            get { return _version; }
        }

        public virtual void UpdateDatabaseAfterUpdateSchema() {

        }
    }
}

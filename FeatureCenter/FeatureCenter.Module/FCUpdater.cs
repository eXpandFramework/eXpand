using System;
using DevExpress.ExpressApp;

namespace FeatureCenter.Module {
    public class FCUpdater {
        readonly IObjectSpace _objectSpace;
        readonly Version _version;
        readonly Xpand.Persistent.BaseImpl.Updater _updater;

        public FCUpdater(IObjectSpace objectSpace, Version version, Xpand.Persistent.BaseImpl.Updater updater) {
            _objectSpace = objectSpace;
            _version = version;
            _updater = updater;
        }

        public Xpand.Persistent.BaseImpl.Updater Updater {
            get { return _updater; }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public Version Version {
            get { return _version; }
        }

        public virtual void UpdateDatabaseAfterUpdateSchema() {
            //            throw new NotImplementedException();
        }
    }
}

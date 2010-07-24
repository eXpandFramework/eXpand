using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    public class ModelDifferenceSyncObject:eXpandCustomObject {
        public ModelDifferenceSyncObject(Session session) : base(session) {
        }

        public override void AfterConstruction() {
            _withUserModel = true;
            _withApplicationModel = true;
        }

        bool _withApplicationModel;

        public bool WithApplicationModel {
            get { return _withApplicationModel; }
            set { SetPropertyValue("WithApplicationModel", ref _withApplicationModel, value); }
        }

        public override string ToString() {
            string ret = null;
            if (WithUserModel)
                ret = "WithUserModel";
            if (WithApplicationModel) {
                if (ret!= null)
                    ret+= " and ";
                ret += "WithApplicationModel";
            }
            return ret;
        }

        bool _withUserModel;

        public bool WithUserModel {
            get { return _withUserModel; }
            set { SetPropertyValue("WithUserModel", ref _withUserModel, value); }
        }
    }
}
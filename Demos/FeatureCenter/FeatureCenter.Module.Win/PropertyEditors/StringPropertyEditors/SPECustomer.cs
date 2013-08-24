using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.PropertyEditors.StringPropertyEditors {
    public class SPECustomer : CustomerBase {
        string _predefine;

        public SPECustomer(Session session)
            : base(session) {
        }

        public string Predefine {
            get { return _predefine; }
            set { SetPropertyValue("Predefine", ref _predefine, value); }
        }
    }
}
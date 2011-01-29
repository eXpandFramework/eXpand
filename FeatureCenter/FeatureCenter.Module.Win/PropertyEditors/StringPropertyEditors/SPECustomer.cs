using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.PropertyEditors.StringPropertyEditors {
    public class SPECustomer : WinCustomer {
        public SPECustomer(Session session)
            : base(session) {
        }
        private string _predefine;

        public string Predefine {
            get {
                return _predefine;
            }
            set {
                SetPropertyValue("Predefine", ref _predefine, value);
            }
        }
    }
}
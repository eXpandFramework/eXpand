using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.PropertyPath {
    public class PPOrder : OrderBase {
        PPCustomer _ppCustomer;
        
        public PPOrder(Session session) : base(session) {
        }

        [Association("PPCustomer-Orders")]
        public PPCustomer PpCustomer
        {
            get { return _ppCustomer; }
            set { SetPropertyValue("PPCustomer", ref _ppCustomer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            PpCustomer=(PPCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return PpCustomer;
        }

        [Association("PPOrder-OrderLines")]
        public XPCollection<PPOrderLine> OrderLines
        {
            get { return GetCollection<PPOrderLine>("OrderLines"); }
        }

    }
}
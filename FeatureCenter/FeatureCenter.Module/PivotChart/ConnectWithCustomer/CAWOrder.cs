using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.PivotChart.ConnectWithCustomer {
    public class CAWOrder:OrderBase {
        public CAWOrder(Session session) : base(session) {
        }
        private CAWCustomer _customer;
        [Association("CAWOrder-CAWOrderLines")]
        public XPCollection<CAWOrderLine> OrderLines
        {
            get
            {
                return GetCollection<CAWOrderLine>("OrderLines");
            }
        }
        [Association("CWCustomer-Orders")]
        public CAWCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }
        protected override void SetCustomer(ICustomer customer) {
            Customer = (CAWCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ControllingSearch {
    public class ControllingSearchOrder : OrderBase {
        ControllingSearchCustomer _customer;

        public ControllingSearchOrder(Session session) : base(session) {
        }

        [Association("ControllingSearchCustomer-ControllingSearchOrders")]
        public ControllingSearchCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (ControllingSearchCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module {


    public class Order : OrderBase {
        Customer _customer;

        public Order(Session session)
            : base(session) {
        }

        [Association("Customer-Orders")]
        public Customer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        [Association("Order-OrderLines")]
        public XPCollection<OrderLine> OrderLines {
            get { return GetCollection<OrderLine>("OrderLines"); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (Customer)customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
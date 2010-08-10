using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.FKViolation {
    public class FKVOrder : OrderBase {
        FKVCustomer _customer;

        public FKVOrder(Session session) : base(session) {
        }

        [ProvidedAssociation("FKVCustomer-FKVOrders")]
        public FKVCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (FKVCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.MultipleDataStore {
    public class MDSOrder : OrderBase {
        MDSCustomer _customer;

        public MDSOrder(Session session) : base(session) {
        }

        [ProvidedAssociation("MDSCustomer-MDSOrders")]
        public MDSCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (MDSCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
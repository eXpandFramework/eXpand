using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.MultipleDataStore {
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
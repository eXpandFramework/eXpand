using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ModelArtifact.ConditionalFKViolationAndShowDetailView {
    public class CFKVOrder:OrderBase
    {
        public CFKVOrder(Session session) : base(session) {
        }
        private CFKVCustomer _customer;
        [ProvidedAssociation("CFKVCustomer-CFKVOrders")]
        public CFKVCustomer Customer{
            get
            {
                return _customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _customer, value);
            }
        }
        protected override void SetCustomer(ICustomer customer) {
            Customer = (CFKVCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
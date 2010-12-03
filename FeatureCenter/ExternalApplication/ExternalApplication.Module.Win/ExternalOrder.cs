using DevExpress.Xpo;
using FeatureCenter.Base;

namespace ExternalApplication.Module.Win {
    
    public class ExternalOrder:OrderBase {
        public ExternalOrder(Session session) : base(session) {
        }
        private ExternalCustomer _customer;

        
        public ExternalCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }
        protected override void SetCustomer(ICustomer customer) {
            Customer = (ExternalCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
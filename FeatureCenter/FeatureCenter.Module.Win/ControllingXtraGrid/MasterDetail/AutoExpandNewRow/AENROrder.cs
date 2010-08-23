using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail.AutoExpandNewRow {
    
    public class AENROrder : OrderBase {
        AENRCustomer _customer;

        public AENROrder(Session session) : base(session) {
        }
        [Association("AENRCustomer-AENROrders")]
        public AENRCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (AENRCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
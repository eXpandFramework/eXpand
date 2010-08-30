using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.PivotChart.ControllingPivotGrid {
    public class CPGOrder : OrderBase {
        CPGCustomer _customer;

        public CPGOrder(Session session) : base(session) {
        }

        [ProvidedAssociation("CPGCustomer-CPGOrders")]
        public CPGCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (CPGCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
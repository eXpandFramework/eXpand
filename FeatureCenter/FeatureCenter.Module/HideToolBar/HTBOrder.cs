using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.HideToolBar {
    public class HTBOrder : OrderBase {
        HTBCustomer _customer;

        public HTBOrder(Session session) : base(session) {
        }

        [ProvidedAssociation("HTBCustomer-HTBOrders")]
        public HTBCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (HTBCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

}
}
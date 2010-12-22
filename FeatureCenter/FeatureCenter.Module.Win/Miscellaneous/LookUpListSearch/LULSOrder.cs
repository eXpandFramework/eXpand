using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.LookUpListSearch {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.ViewMessageLookUpListSearch, Position.Bottom, ViewType =  ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.HeaderLookUpListSearch, Position.Top,ViewType = ViewType.DetailView)]
    public class LULSOrder:OrderBase {
        public LULSOrder(Session session) : base(session) {
        }
        private LULSCustomer _customer;

        [ProvidedAssociation("LULSCustomer-LULSOrders")]
        public LULSCustomer Customer {
            get { return _customer; }
            set { SetPropertyValue("Customer", ref _customer, value); }
        }
        protected override void SetCustomer(ICustomer customer) {
            Customer=(LULSCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }

    }
}
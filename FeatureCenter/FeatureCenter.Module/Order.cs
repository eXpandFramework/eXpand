using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module
{
    #region LookUp ListView Search
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.ViewMessageLookUpListSearch, Position.Bottom, ViewType = ViewType.DetailView, View = "LookUpListViewSearch_DetailView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderLookUpListSearch, "1=1", "1=1", Captions.HeaderLookUpListSearch, Position.Top, ViewType = ViewType.DetailView, View = "LookUpListViewSearch_DetailView")]
    [NavigationItem(Captions.Miscellaneous+"Lookup ListView Search", "LookUpListViewSearch_DetailView")]
    [CloneView(CloneViewType.DetailView, "LookUpListViewSearch_DetailView")]
    [DisplayFeatureModelAttribute("LookUpListViewSearch_DetailView")]
    #endregion
    #region Hide Nested ListView ToolBar
    [CloneView(CloneViewType.ListView, "HideNestedListToolBarView_ListView")]
    #endregion

    public class Order : OrderBase {
        Customer _customer;

        public Order(Session session) : base(session) {
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
            Customer = (Customer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
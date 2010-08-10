using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.MasterDetail.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win
{
    #region Conditional Master Detail Views
    [CloneView(CloneViewType.ListView, "ConditionalMasterDetailOrderForParis_ListView")]
    [CloneView(CloneViewType.ListView, "ConditionalMasterDetailOrder_ListView")]
    #endregion

    #region Master Detail At Any Level
    [CloneView(CloneViewType.ListView, "MasterDetailAtAnyLevelOrder_ListView")]
    [MasterDetail("AtAnyLevelOrder_OrderLines", "1=1", "MasterDetailAtAnyLevelOrderLine_ListView", "OrderLines", View = "MasterDetailAtAnyLevelOrder_ListView")]
    #endregion
    #region Auto Expand New Row
    [CloneView(CloneViewType.ListView, "AutoExpandNewRowOrder_ListView")]
    #endregion
    public class WinOrder : OrderBase {
        WinCustomer _winCustomer;

        public WinOrder(Session session) : base(session) {
        }

        [Association("WinOrder-WinOrderLines")]
        public XPCollection<WinOrderLine> OrderLines {
            get { return GetCollection<WinOrderLine>("OrderLines"); }
        }

        [Association("WinCustomer-WinOrders")]
        public WinCustomer Customer {
            get { return _winCustomer; }
            set { SetPropertyValue("Customer", ref _winCustomer, value); }
        }

        protected override void SetCustomer(ICustomer customer) {
            Customer = (WinCustomer) customer;
        }

        protected override ICustomer GetCustomer() {
            return Customer;
        }
    }
}
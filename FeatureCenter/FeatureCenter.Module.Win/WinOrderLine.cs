using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win
{
    

    #region Master Detail At Any Level
    [CloneView(CloneViewType.ListView, "MasterDetailAtAnyLevelOrderLine_ListView")]
    #endregion
    public class WinOrderLine:OrderLineBase {
        public WinOrderLine(Session session) : base(session) {
        }
        private WinOrder _winOrder;

        [Association("WinOrder-WinOrderLines")]
        public WinOrder Order {
            get { return _winOrder; }
            set { SetPropertyValue("Order", ref _winOrder, value); }
        }
        protected override void SetOrder(IOrder order) {
            Order = (WinOrder)order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
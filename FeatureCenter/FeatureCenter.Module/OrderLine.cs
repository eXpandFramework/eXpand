using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module {
    
    [VisibleInReports(true)]
    public class OrderLine : OrderLineBase {
        Order _order;

        public OrderLine(Session session) : base(session) {
        }

        [Association("Order-OrderLines")]
        public Order Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (Order) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
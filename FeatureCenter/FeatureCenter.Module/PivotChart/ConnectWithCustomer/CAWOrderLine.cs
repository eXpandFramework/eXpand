using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.PivotChart.ConnectWithCustomer {
    [VisibleInReports]
    public class CAWOrderLine : OrderLineBase {
        CAWOrder _order;

        public CAWOrderLine(Session session) : base(session) {
        }

        [Association("CAWOrder-CAWOrderLines")]
        public CAWOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }
        
        
        
        protected override void SetOrder(IOrder order) {
            Order = (CAWOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
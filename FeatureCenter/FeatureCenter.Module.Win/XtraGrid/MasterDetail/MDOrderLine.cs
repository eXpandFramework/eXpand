using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail {
    public class MDOrderLine:OrderLineBase {
        public MDOrderLine(Session session) : base(session) {
        }
        private MDOrder _order;

        [Association("MDOrder-MDOrderLines")]
        public MDOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }
        protected override void SetOrder(IOrder order) {
            Order = (MDOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
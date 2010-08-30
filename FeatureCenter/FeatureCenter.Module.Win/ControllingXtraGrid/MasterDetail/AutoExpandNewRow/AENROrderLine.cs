using DevExpress.Xpo;
using FeatureCenter.Module.BaseObjects;

namespace FeatureCenter.Module.Win.MasterDetail.AutoExpandNewRow {
    public class AENROrderLine : OrderLineBase {
        AENROrder _order;

        public AENROrderLine(Session session) : base(session) {
        }

        [Association("AENROrder-AENROrderLines")]
        public AENROrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (AENROrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
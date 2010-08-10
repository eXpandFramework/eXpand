using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ControllingSearch {
    public class ControllingSearchOrderLine : OrderLineBase {
        ControllingSearchOrder _order;

        public ControllingSearchOrderLine(Session session) : base(session) {
        }

        [ProvidedAssociation("ControllingSearchOrder-ControllingSearchOrderLines")]
        public ControllingSearchOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (ControllingSearchOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
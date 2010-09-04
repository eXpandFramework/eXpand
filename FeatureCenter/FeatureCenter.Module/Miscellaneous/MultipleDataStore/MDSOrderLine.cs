using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.MultipleDataStore {
    public class MDSOrderLine : OrderLineBase {
        MDSOrder _order;

        public MDSOrderLine(Session session) : base(session) {
        }

        [ProvidedAssociation("MDSOrder-MDSOrderLines")]
        public MDSOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (MDSOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
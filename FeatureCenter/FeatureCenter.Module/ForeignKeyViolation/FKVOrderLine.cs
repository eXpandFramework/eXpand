using System;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.FKViolation {
    public class FKVOrderLine : OrderLineBase {
        FKVOrder _order;

        public FKVOrderLine(Session session) : base(session) {
        }

        [ProvidedAssociation("FKVOrder-FKVOrderLines")]
        public FKVOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (FKVOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
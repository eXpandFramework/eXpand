using System;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.MasterDetail.ConditionalDetailViews {
    public class MDCDVOrderLine : OrderLineBase {
        MDCDVOrder _order;

        public MDCDVOrderLine(Session session) : base(session) {
        }

        [Association("MDCDVOrder-MDCDVOrderLines")]
        public MDCDVOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (MDCDVOrder) order;
        }

        protected override IOrder GetOrder() {
            throw new NotImplementedException();
        }
    }
}
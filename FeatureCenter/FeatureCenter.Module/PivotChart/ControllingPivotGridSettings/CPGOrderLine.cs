using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.PivotChart.ControllingPivotGrid {
    [VisibleInReports(true)]
    public class CPGOrderLine : OrderLineBase {
        CPGOrder _order;

        public CPGOrderLine(Session session) : base(session) {
        }

        [ProvidedAssociation("CPGOrder-CPGOrderLines")]
        public CPGOrder Order {
            get { return _order; }
            set { SetPropertyValue("Order", ref _order, value); }
        }

        protected override void SetOrder(IOrder order) {
            Order = (CPGOrder) order;
        }

        protected override IOrder GetOrder() {
            return Order;
        }
    }
}
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.ListViewControl.PropertyPathFilters {
    
    
    public class PPOrderLine : OrderLineBase {
        PPOrder _ppOrder;

        public PPOrderLine(Session session) : base(session) {
        }

        [Association("PPOrder-OrderLines")]
        public PPOrder PpOrder {
            get { return _ppOrder; }
            set { SetPropertyValue("PPOrder", ref _ppOrder, value); }
        }

        protected override void SetOrder(IOrder order) {
            PpOrder=(PPOrder) order;
        }

        protected override IOrder GetOrder() {
            return PpOrder;
        }
    }
}
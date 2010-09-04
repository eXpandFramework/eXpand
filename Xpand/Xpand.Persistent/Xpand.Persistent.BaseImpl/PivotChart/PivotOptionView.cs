using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionView : BaseObject, IPivotOptionView {
        public PivotOptionView(Session session) : base(session) {
        }
    }
}
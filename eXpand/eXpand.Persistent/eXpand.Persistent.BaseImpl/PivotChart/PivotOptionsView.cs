using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsView : BaseObject, IPivotOptionsView {
        public PivotOptionsView(Session session) : base(session) {
        }
    }
}
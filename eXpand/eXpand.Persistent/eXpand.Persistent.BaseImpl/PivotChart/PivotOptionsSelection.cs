using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsSelection : BaseObject, IPivotOptionsSelection {
        public PivotOptionsSelection(Session session) : base(session) {
        }
    }
}
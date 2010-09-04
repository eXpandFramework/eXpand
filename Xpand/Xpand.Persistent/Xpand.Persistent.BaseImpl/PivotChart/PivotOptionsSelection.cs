using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsSelection : BaseObject, IPivotOptionsSelection {
        public PivotOptionsSelection(Session session) : base(session) {
        }
    }
}
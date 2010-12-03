using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsSelection : XpandBaseCustomObject, IPivotOptionsSelection {
        public PivotOptionsSelection(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsSelection : XpandCustomObject, IPivotOptionsSelection {
        public PivotOptionsSelection(Session session)
            : base(session) {
        }
    }
}
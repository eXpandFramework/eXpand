using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsFilterPopup : XpandBaseCustomObject, IPivotOptionsFilterPopup {
        public PivotOptionsFilterPopup(Session session)
            : base(session) {
        }
    }
}
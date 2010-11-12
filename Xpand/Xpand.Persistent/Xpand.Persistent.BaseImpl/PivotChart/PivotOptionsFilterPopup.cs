using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsFilterPopup : XpandCustomObject, IPivotOptionsFilterPopup {
        public PivotOptionsFilterPopup(Session session)
            : base(session) {
        }
    }
}
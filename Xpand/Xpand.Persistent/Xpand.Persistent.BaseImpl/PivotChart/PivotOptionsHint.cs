using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsHint : XpandCustomObject, IPivotOptionsHint {
        public PivotOptionsHint(Session session)
            : base(session) {
        }
    }
}
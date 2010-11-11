using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsBehavior : XpandCustomObject, IPivotOptionsBehavior {
        public PivotOptionsBehavior(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsBehavior : XpandBaseCustomObject, IPivotOptionsBehavior {
        public PivotOptionsBehavior(Session session)
            : base(session) {
        }
    }
}
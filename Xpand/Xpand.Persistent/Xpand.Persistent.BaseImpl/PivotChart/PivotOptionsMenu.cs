using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsMenu : XpandBaseCustomObject, IPivotOptionsMenu {
        public PivotOptionsMenu(Session session)
            : base(session) {
        }
    }
}
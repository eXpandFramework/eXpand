using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsMenu : XpandCustomObject, IPivotOptionsMenu {
        public PivotOptionsMenu(Session session)
            : base(session) {
        }
    }
}
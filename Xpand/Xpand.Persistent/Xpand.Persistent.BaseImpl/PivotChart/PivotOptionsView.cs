using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsView : XpandCustomObject, IPivotOptionsView {
        public PivotOptionsView(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsView : XpandBaseCustomObject, IPivotOptionsView {
        public PivotOptionsView(Session session)
            : base(session) {
        }
    }
}
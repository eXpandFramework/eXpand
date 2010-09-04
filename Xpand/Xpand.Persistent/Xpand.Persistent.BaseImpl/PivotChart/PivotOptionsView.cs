using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsView : BaseObject, IPivotOptionsView {
        public PivotOptionsView(Session session) : base(session) {
        }
    }
}
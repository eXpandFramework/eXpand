using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsData : XpandBaseCustomObject, IPivotOptionsData {
        public PivotOptionsData(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsData : XpandCustomObject, IPivotOptionsData {
        public PivotOptionsData(Session session)
            : base(session) {
        }
    }
}
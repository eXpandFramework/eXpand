using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsData : BaseObject, IPivotOptionsData {
        public PivotOptionsData(Session session) : base(session) {
        }
    }
}
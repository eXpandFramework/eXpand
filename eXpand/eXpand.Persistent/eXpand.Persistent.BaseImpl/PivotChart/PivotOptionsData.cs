using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsData : BaseObject, IPivotOptionsData {
        public PivotOptionsData(Session session) : base(session) {
        }
    }
}
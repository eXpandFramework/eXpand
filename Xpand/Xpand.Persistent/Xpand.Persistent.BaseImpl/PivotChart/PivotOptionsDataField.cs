using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsDataField : XpandCustomObject, IPivotOptionsDataField {
        public PivotOptionsDataField(Session session)
            : base(session) {
        }
    }
}
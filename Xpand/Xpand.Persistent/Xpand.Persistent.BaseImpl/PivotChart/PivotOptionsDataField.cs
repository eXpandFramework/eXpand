using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsDataField : XpandBaseCustomObject, IPivotOptionsDataField {
        public PivotOptionsDataField(Session session)
            : base(session) {
        }
    }
}
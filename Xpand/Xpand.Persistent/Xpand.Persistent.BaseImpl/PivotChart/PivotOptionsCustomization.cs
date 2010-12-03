using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent]
    public class PivotOptionsCustomization : XpandBaseCustomObject, IPivotOptionsCustomization {
        public PivotOptionsCustomization(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsCustomization : XpandCustomObject, IPivotOptionsCustomization {
        public PivotOptionsCustomization(Session session)
            : base(session) {
        }
    }
}
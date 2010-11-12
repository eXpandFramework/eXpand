using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsCustomization : XpandCustomObject, IPivotGridWebOptionsCustomization {
        public PivotGridWebOptionsCustomization(Session session)
            : base(session) {
        }
    }
}
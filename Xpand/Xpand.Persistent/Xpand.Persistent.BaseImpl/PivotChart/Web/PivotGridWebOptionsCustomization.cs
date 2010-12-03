using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsCustomization : XpandBaseCustomObject, IPivotGridWebOptionsCustomization {
        public PivotGridWebOptionsCustomization(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsPager : XpandCustomObject, IPivotGridWebOptionsPager {
        public PivotGridWebOptionsPager(Session session)
            : base(session) {
        }
    }
}
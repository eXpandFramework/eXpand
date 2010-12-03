using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsPager : XpandBaseCustomObject, IPivotGridWebOptionsPager {
        public PivotGridWebOptionsPager(Session session)
            : base(session) {
        }
    }
}
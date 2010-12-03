using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsView : XpandBaseCustomObject, IPivotGridWebOptionsView {
        public PivotGridWebOptionsView(Session session)
            : base(session) {
        }
    }
}
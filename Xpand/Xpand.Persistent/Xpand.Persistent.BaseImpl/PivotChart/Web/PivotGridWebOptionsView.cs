using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsView : XpandCustomObject, IPivotGridWebOptionsView {
        public PivotGridWebOptionsView(Session session)
            : base(session) {
        }
    }
}
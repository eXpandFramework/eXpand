using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsLoadingPanel : XpandBaseCustomObject, IPivotGridWebOptionsLoadingPanel {
        public PivotGridWebOptionsLoadingPanel(Session session)
            : base(session) {
        }
    }
}
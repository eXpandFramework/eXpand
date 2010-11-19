using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsLoadingPanel : XpandCustomObject, IPivotGridWebOptionsLoadingPanel {
        public PivotGridWebOptionsLoadingPanel(Session session)
            : base(session) {
        }
    }
}
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsLoadingPanel : BaseObject, IPivotGridWebOptionsLoadingPanel
    {
        public PivotGridWebOptionsLoadingPanel(Session session) : base(session) {
        }
    }
}
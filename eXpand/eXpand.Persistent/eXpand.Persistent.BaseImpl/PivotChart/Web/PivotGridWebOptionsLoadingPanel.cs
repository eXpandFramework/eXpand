using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsLoadingPanel : BaseObject, IPivotGridWebOptionsLoadingPanel
    {
        public PivotGridWebOptionsLoadingPanel(Session session) : base(session) {
        }
    }
}
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsView : BaseObject, IPivotGridWebOptionsView
    {
        public PivotGridWebOptionsView(Session session) : base(session) {
        }
    }
}
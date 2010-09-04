using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsView : BaseObject, IPivotGridWebOptionsView
    {
        public PivotGridWebOptionsView(Session session) : base(session) {
        }
    }
}
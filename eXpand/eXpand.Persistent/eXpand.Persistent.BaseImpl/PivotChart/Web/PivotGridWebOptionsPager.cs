using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsPager : BaseObject, IPivotGridWebOptionsPager
    {
        public PivotGridWebOptionsPager(Session session) : base(session) {
        }
    }
}
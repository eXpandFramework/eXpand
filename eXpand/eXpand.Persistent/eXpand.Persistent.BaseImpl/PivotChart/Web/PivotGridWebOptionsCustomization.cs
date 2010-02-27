using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsCustomization : BaseObject, IPivotGridWebOptionsCustomization
    {
        public PivotGridWebOptionsCustomization(Session session) : base(session) {
        }
    }
}
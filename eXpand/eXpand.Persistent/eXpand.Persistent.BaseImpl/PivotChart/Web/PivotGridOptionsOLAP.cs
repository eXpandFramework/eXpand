using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridOptionsOLAP : BaseObject,IPivotGridOptionsOLAP {
        public PivotGridOptionsOLAP(Session session) : base(session) {
        }
    }
}
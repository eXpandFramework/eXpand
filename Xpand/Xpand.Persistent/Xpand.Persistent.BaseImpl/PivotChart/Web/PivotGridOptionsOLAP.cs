using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridOptionsOLAP : BaseObject,IPivotGridOptionsOLAP {
        public PivotGridOptionsOLAP(Session session) : base(session) {
        }
    }
}
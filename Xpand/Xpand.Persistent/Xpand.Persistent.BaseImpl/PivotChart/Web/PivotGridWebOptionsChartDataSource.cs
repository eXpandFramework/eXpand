using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsChartDataSource : XpandBaseCustomObject, IPivotGridWebOptionsChartDataSource {
        public PivotGridWebOptionsChartDataSource(Session session)
            : base(session) {
        }
    }
}
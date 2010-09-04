using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsChartDataSource : BaseObject, IPivotGridWebOptionsChartDataSource
    {
        public PivotGridWebOptionsChartDataSource(Session session) : base(session) {
        }
    }
}
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridWebOptionsChartDataSource : BaseObject, IPivotGridWebOptionsChartDataSource
    {
        public PivotGridWebOptionsChartDataSource(Session session) : base(session) {
        }
    }
}
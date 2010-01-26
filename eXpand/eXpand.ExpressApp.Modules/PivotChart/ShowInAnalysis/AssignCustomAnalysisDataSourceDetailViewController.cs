using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.ShowInAnalysis {
    public class AssignCustomAnalysisDataSourceDetailViewController : AnalysisViewControllerBase {
        readonly CriteriaOperator selectionCriteria;


        public AssignCustomAnalysisDataSourceDetailViewController() {
        }


        public AssignCustomAnalysisDataSourceDetailViewController(CriteriaOperator criteria) {
            selectionCriteria = criteria;
        }

        protected override void OnActivated() {
            base.OnActivated();
            analysisEditor.DataSourceCreating += analysisEditor_DataSourceCreating;
        }

        void analysisEditor_DataSourceCreating(object sender, DataSourceCreatingEventArgs e) {
            CriteriaOperator userCriteria = null;
            if (e.AnalysisInfo != null) {
                if (!string.IsNullOrEmpty(e.AnalysisInfo.Criteria)) {
                    userCriteria = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(e.AnalysisInfo.Criteria,e.AnalysisInfo.DataType);
                }
                e.DataSource = View.ObjectSpace.CreateCollection(e.AnalysisInfo.DataType,userCriteria & selectionCriteria);
                e.Handled = true;
            }            
        }
    }
}
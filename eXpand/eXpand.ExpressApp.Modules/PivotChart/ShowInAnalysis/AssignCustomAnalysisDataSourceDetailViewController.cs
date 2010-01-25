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
            // This event is fired when assigning a data source to the editor's control. So, you can handle it to provide your own data source.
            analysisEditor.DataSourceCreating += analysisEditor_DataSourceCreating;
        }

        void analysisEditor_DataSourceCreating(object sender, DataSourceCreatingEventArgs e) {
            CriteriaOperator userCriteria = null;
            if (!string.IsNullOrEmpty(e.AnalysisInfo.Criteria)) {
                // This is a wrapper class that parses the XAF's "native" criteria strings and converts "these XAF's things" by replacing them with actual values into a normal XPO criteria string.
                userCriteria = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(e.AnalysisInfo.Criteria,
                                                                                   e.AnalysisInfo.DataType);
            }
            // After that you can reach the normal (understandable to XPO) criteria and use it to filter a collection.
            e.DataSource = View.ObjectSpace.CreateCollection(e.AnalysisInfo.DataType, userCriteria & selectionCriteria);
            // We need to set this parameter to true to notify that we completely handled the event and provided the data source to the editor's control.
            e.Handled = true;
        }
    }
}
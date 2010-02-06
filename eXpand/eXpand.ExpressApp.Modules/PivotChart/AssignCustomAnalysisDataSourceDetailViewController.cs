using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart {
    public class AssignCustomAnalysisDataSourceDetailViewController : AnalysisViewControllerBase {
        readonly CriteriaOperator _criteriaOperator;
        public event EventHandler<DataSourceCreatingEventArgs> DataSourceAssigned;
        public event EventHandler<CriteriaOperatorArgs> ApplyingCollectionCriteria;

        protected virtual void InvokeApplyingCollectionCriteria(CriteriaOperatorArgs e) {
            EventHandler<CriteriaOperatorArgs> handler = ApplyingCollectionCriteria;
            if (handler != null) handler(this, e);
        }

        protected virtual void InvokeDataSourceAssigned(DataSourceCreatingEventArgs e) {
            EventHandler<DataSourceCreatingEventArgs> handler = DataSourceAssigned;
            if (handler != null) handler(this, e);
        }


        public AssignCustomAnalysisDataSourceDetailViewController() {
        }

        public AssignCustomAnalysisDataSourceDetailViewController(CriteriaOperator criteriaOperator) {
            _criteriaOperator = criteriaOperator;
        }

        protected override void OnActivated() {
            base.OnActivated();
            foreach (var analysisEditor in AnalysisEditors) {
                analysisEditor.DataSourceCreating += analysisEditor_DataSourceCreating;    
            }
        }

        void analysisEditor_DataSourceCreating(object sender, DataSourceCreatingEventArgs e) {
            CriteriaOperator userCriteria = null;
            
            if (e.AnalysisInfo != null) {
                if (!string.IsNullOrEmpty(e.AnalysisInfo.Criteria)) {
                    userCriteria = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(e.AnalysisInfo.Criteria,e.AnalysisInfo.DataType);
                }

                var criteriaOperatorArgs = new CriteriaOperatorArgs((AnalysisEditorBase)sender);
                InvokeApplyingCollectionCriteria(criteriaOperatorArgs);
                e.DataSource = View.ObjectSpace.CreateCollection(e.AnalysisInfo.DataType, userCriteria & criteriaOperatorArgs.Criteria & _criteriaOperator);
                e.Handled = true;
                InvokeDataSourceAssigned(e);
            }            
        }


    }

    public class CriteriaOperatorArgs : EventArgs {
        readonly AnalysisEditorBase _analysisEditorBase;

        public CriteriaOperatorArgs(AnalysisEditorBase analysisEditorBase) {
            _analysisEditorBase = analysisEditorBase;
        }

        public AnalysisEditorBase AnalysisEditorBase {
            get { return _analysisEditorBase; }
        }

        public CriteriaOperator Criteria { get; set; }
    }

    public delegate void DataSourceAssignedEventHandler(object sender, DataSourceCreatingEventArgs args);

}
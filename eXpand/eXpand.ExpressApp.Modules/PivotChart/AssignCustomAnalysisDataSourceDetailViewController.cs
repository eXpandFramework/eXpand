using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart {
    public class AssignCustomAnalysisDataSourceDetailViewController : AnalysisViewControllerBase {
        readonly CriteriaOperator _criteriaOperator;
        readonly Dictionary<AnalysisEditorBase, CriteriaOperator> operators = new Dictionary<AnalysisEditorBase, CriteriaOperator>();
        public event EventHandler<DataSourceCreatingEventArgs> DataSourceAssigned;

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
                CriteriaOperator criteriaOperator= null;
                if (operators.Count>0) {
                    criteriaOperator = operators[(AnalysisEditorBase)sender];    
                }
                e.DataSource = View.ObjectSpace.CreateCollection(e.AnalysisInfo.DataType,userCriteria & criteriaOperator&_criteriaOperator);
                e.Handled = true;
                InvokeDataSourceAssigned(e);
            }            
        }


        public void SetCriteria(IMemberInfo memberInfo, CriteriaOperator criteriaOperator) {
            operators.Add(AnalysisEditors.Where(@base => @base.MemberInfo.Name.StartsWith(memberInfo.Name)).Single(), criteriaOperator);
        }
    }

    public delegate void DataSourceAssignedEventHandler(object sender, DataSourceCreatingEventArgs args);

}
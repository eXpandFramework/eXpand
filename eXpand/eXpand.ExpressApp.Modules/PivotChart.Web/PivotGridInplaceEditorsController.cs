using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Xpo;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart.Web {
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase {
        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase) {
            var pivotGridControl = ((AnalysisControlWeb) (analysisEditorBase).Control).PivotGrid;
            pivotGridControl.ClientInstanceName = analysisEditorBase.MemberInfo.Name.Replace(".","_");
//            var pivotDrillDownDataSource = pivotGridControl.CreateDrillDownDataSource(4, 0);
//            pivotDrillDownDataSource[0][4] = 55;
//            pivotGridControl.ClientInstanceName = "pivot";
            pivotGridControl.CustomCallback+=PivotGridControlOnCustomCallback;
            pivotGridControl.CellTemplate = new CellTemplate(pivotGridControl.ClientInstanceName);
//            pivotGridControl.CustomCellStyle+=PivotGridControlOnCustomCellStyle;            
//            pivotGridControl.ClientSideEvents.CellClick = "function(s, e) {alert('');}";
//            pivotGridControl.ClientInstanceName = "pivot";
        }

        void PivotGridControlOnCustomCallback(object sender, PivotGridCustomCallbackEventArgs pivotGridCustomCallbackEventArgs) {
            var args = pivotGridCustomCallbackEventArgs.Parameters.Split('|');
            var columnIndex = Convert.ToInt32(args[1]);
            
            var rowIndex = Convert.ToInt32(args[2]);
            var value = Convert.ToDouble(args[3]);
            var asPxPivotGrid = ((ASPxPivotGrid)sender);
            var pivotDrillDownDataSource = asPxPivotGrid.CreateDrillDownDataSource(columnIndex, rowIndex);
            for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++) {
                pivotDrillDownDataSource[i][args[0]] = Convert.ToDouble(value);
            }

        }
    }

    public class CellTemplate : ITemplate {
        readonly string _clientInstanceName;

        public CellTemplate(string clientInstanceName) {
            _clientInstanceName = clientInstanceName;
        }
        #region ITemplate Members
        public void InstantiateIn(Control container) {

            var c = (PivotGridCellTemplateContainer) container;
//            DevExpress.XtraPivotGrid.Data.PivotGridData data = fieldValue.ValueItem.Data;
//            int cindex = (fieldValue.Field.IsColumn) ? fieldValue.ValueItem.VisibleIndex : -1;
//            int rindex = (fieldValue.Field.IsColumn) ? -1 : fieldValue.ValueItem.VisibleIndex;

            var asPxSpinEdit = new ASPxSpinEdit {Text = c.Text, Width = Unit.Percentage(100)};
            asPxSpinEdit.SpinButtons.ShowIncrementButtons= false;
            asPxSpinEdit.EnableClientSideAPI = true;
            var columnIndex = ((PivotGridCellTemplateItem)(c.DataItem)).ColumnIndex;
            var rowIndex = ((PivotGridCellTemplateItem)(c.DataItem)).RowIndex;
//            asPxSpinEdit.AutoPostBack = true;
            asPxSpinEdit.ClientInstanceName = string.Format("{0}{1}_{2}", _clientInstanceName, columnIndex, rowIndex);
//            asPxSpinEdit.TextChanged+=AsPxSpinEditOnValueChanged;
//            asPxSpinEdit.ClientInstanceName = "editor";
            //pivot.PerformCallback(&quot;D|&quot; + colIndex + &quot;|&quot; + rowIndex + &quot;|&quot; + editor.GetText());
            asPxSpinEdit.ClientSideEvents.ValueChanged =
                "function (s,e){var editorText=" + asPxSpinEdit.ClientInstanceName + ".GetText();"+_clientInstanceName + ".PerformCallback('"+c.DataField.FieldName+"|"+ columnIndex + "|"+rowIndex+"|'+editorText)}";
//                string.Format("function (s,e){{{0}.PerformCallback(&quot;D|&quot; + {1} + &quot;|&quot; + {2} + &quot;|&quot; + {3}.GetText()));}}",
//                                 _clientInstanceName, columnIndex, rowIndex, asPxSpinEdit.ClientInstanceName);
//            asPxSpinEdit.ClientSideEvents.TextChanged = asPxSpinEdit.ClientSideEvents.ValueChanged;            
            c.Controls.Add(asPxSpinEdit);
        }

        void AsPxSpinEditOnValueChanged(object sender, EventArgs eventArgs) {
            
        }

        string ClientInstanceName(PivotGridCellTemplateContainer pivotGridCellTemplateContainer) {
            string clientInstanceName = null;
            try {
                clientInstanceName = pivotGridCellTemplateContainer.ColumnField.DataControllerColumnName + pivotGridCellTemplateContainer.RowField.DataControllerColumnName + pivotGridCellTemplateContainer.DataField.DataControllerColumnName;
                clientInstanceName = clientInstanceName.Replace(".", "_");
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            return clientInstanceName;
        }
        #endregion
    }
}
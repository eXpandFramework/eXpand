using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Web
{
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase
    {
        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase)
        {
            var pivotGridControl = ((AnalysisControlWeb)(analysisEditorBase).Control).PivotGrid;
            pivotGridControl.ClientInstanceName = analysisEditorBase.MemberInfo.Name.Replace(".", "_");
            pivotGridControl.CustomCallback += PivotGridControlOnCustomCallback;
            pivotGridControl.CellTemplate = new CellTemplate(pivotGridControl.ClientInstanceName);
        }

        void PivotGridControlOnCustomCallback(object sender, PivotGridCustomCallbackEventArgs pivotGridCustomCallbackEventArgs)
        {
            var args = pivotGridCustomCallbackEventArgs.Parameters.Split('|');
            var columnIndex = Convert.ToInt32(args[1]);

            var rowIndex = Convert.ToInt32(args[2]);
            var value = Convert.ToDouble(args[3]);
            var asPxPivotGrid = ((ASPxPivotGrid)sender);
            var pivotDrillDownDataSource = asPxPivotGrid.CreateDrillDownDataSource(columnIndex, rowIndex);
            for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++)
            {
                pivotDrillDownDataSource[i][args[0]] = Convert.ToDouble(value);
            }


        }

        public class CellTemplate : ITemplate
        {
            readonly string _clientInstanceName;

            public CellTemplate(string clientInstanceName)
            {
                _clientInstanceName = clientInstanceName;
            }
            #region ITemplate Members
            public void InstantiateIn(Control container)
            {

                var c = (PivotGridCellTemplateContainer)container;
                var asPxSpinEdit = new ASPxSpinEdit { Text = c.Text, Width = Unit.Percentage(100) };
                asPxSpinEdit.SpinButtons.ShowIncrementButtons = false;
                asPxSpinEdit.EnableClientSideAPI = true;
                var columnIndex = ((PivotGridCellTemplateItem)(c.DataItem)).ColumnIndex;
                var rowIndex = ((PivotGridCellTemplateItem)(c.DataItem)).RowIndex;
                asPxSpinEdit.ClientInstanceName = string.Format("{0}{1}_{2}", _clientInstanceName, columnIndex, rowIndex);
                asPxSpinEdit.ID = string.Format("Ec{0}r{1}", columnIndex, rowIndex);
                asPxSpinEdit.ClientSideEvents.ValueChanged =
                    "function (s,e){var editorText=" + asPxSpinEdit.ClientInstanceName + ".GetText();" + _clientInstanceName + ".PerformCallback('" + c.DataField.FieldName + "|" + columnIndex + "|" + rowIndex + "|'+editorText)}";
                c.Controls.Add(asPxSpinEdit);
            }
            #endregion
        }
    }
}
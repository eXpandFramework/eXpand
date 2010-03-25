using System;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Web.ASPxPivotGrid;
using eXpand.ExpressApp.PivotChart.InPlaceEdit;
using eXpand.ExpressApp.PivotChart.Web.Editors;

namespace eXpand.ExpressApp.PivotChart.Web.InPlaceEdit
{
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase
    {
        
        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase)
        {
            var pivotGridControl = ((AnalysisControlWeb)(analysisEditorBase).Control).PivotGrid;
            pivotGridControl.ClientInstanceName = analysisEditorBase.MemberInfo.Name.Replace(".", "_");
            pivotGridControl.CustomCallback += PivotGridControlOnCustomCallback;
            pivotGridControl.CellTemplate = new CellTemplate(pivotGridControl.ClientInstanceName);
            pivotGridControl.ClientSideEvents.AfterCallback = "function(s,e){" +
                "if(!window.editorToFocus) return;" +
                "var ed = ASPxClientControl.GetControlCollection().Get(window.editorToFocus);" +
                "if(ed == null) return;" +
                "ed.Focus();" +
                "ed.SelectAll();"+                
                "}";
        }

        void PivotGridControlOnCustomCallback(object sender, PivotGridCustomCallbackEventArgs pivotGridCustomCallbackEventArgs)
        {
            var args = pivotGridCustomCallbackEventArgs.Parameters.Split('|');
            var columnIndex = Convert.ToInt32(args[0]);
            var rowIndex = Convert.ToInt32(args[1]);
            var value = Convert.ToDouble(args[2]);
            var asPxPivotGrid = ((ASPxPivotGrid)sender);
            var pivotDrillDownDataSource = asPxPivotGrid.CreateDrillDownDataSource(columnIndex, rowIndex);
            for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++)
            {
                pivotDrillDownDataSource[i]["Amount"] = Convert.ToDouble(value);
            }
        }
    }
}
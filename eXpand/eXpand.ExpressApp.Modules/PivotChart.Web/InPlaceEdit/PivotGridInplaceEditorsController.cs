using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxHiddenField;
using DevExpress.Web.ASPxPivotGrid;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.PivotChart.InPlaceEdit;
using eXpand.ExpressApp.PivotChart.Web.Editors;

namespace eXpand.ExpressApp.PivotChart.Web.InPlaceEdit
{
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase
    {
        private const string UseCallBackAttribute = "UseCallBack";
        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase)
        {
            var pivotGridControl = ((AnalysisControlWeb)(analysisEditorBase).Control).PivotGrid;
            string memberName = analysisEditorBase.MemberInfo.Name.Replace(".", "_");
            ASPxSpinEdit asPxSpinEdit = new ASPxSpinEdit();
            pivotGridControl.Parent.Controls.Add(asPxSpinEdit);
            pivotGridControl.ClientInstanceName = memberName;
            bool useCallBack = analysisEditorBase.Info.GetAttributeBoolValue(UseCallBackAttribute, true);
            if (useCallBack)
                pivotGridControl.CustomCallback += PivotGridControlOnCustomCallback;


//            var asPxHiddenField = new ASPxSpinEdit();
            Frame.GetController<WebDetailViewController>().SaveAction.Execute += (o, eventArgs) => updateValues(asPxSpinEdit, pivotGridControl);
            pivotGridControl.CellTemplate = new CellTemplate(memberName, useCallBack, asPxSpinEdit);
            
        }

        void updateValues(ASPxSpinEdit hiddenField, ASPxPivotGrid pivotGridControl) {
            
        }

        void PivotGridControlOnCustomCallback(object sender, PivotGridCustomCallbackEventArgs pivotGridCustomCallbackEventArgs)
        {
            var args = pivotGridCustomCallbackEventArgs.Parameters.Split('|');
            var columnIndex = Convert.ToInt32(args[1]);

            var rowIndex = Convert.ToInt32(args[2]);
            var value = Convert.ToDouble(args[3]);
            var asPxPivotGrid = ((ASPxPivotGrid)sender);
            var pivotDrillDownDataSource = asPxPivotGrid.CreateDrillDownDataSource(columnIndex, rowIndex);
            for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++){
                pivotDrillDownDataSource[i][args[0]] = Convert.ToDouble(value);
            }


        }
        public override Schema GetSchema()
        {
            DictionaryNode dictionaryNode = new SchemaHelper().InjectBoolAttribute(UseCallBackAttribute,ModelElement.DetailViewPropertyEditors);
            return new Schema(dictionaryNode);
        }
    }
}
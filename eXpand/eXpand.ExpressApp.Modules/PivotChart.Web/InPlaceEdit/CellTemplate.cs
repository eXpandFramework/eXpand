using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Web.InPlaceEdit {
    public class CellTemplate : ITemplate
    {
        readonly string _memberName;
        readonly bool _useCallBack;
        

        public CellTemplate(string memberName, bool useCallBack) {
            _memberName = memberName;
            _useCallBack = useCallBack;
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
            asPxSpinEdit.ClientInstanceName = String.Format("{0}{1}_{2}", _memberName, columnIndex, rowIndex);
            asPxSpinEdit.ID = String.Format("Ec{0}r{1}", columnIndex, rowIndex);
            if (_useCallBack)
                asPxSpinEdit.ClientSideEvents.ValueChanged ="function (s,e){var editorText=" + asPxSpinEdit.ClientInstanceName + ".GetText();" + _memberName +
                    ".PerformCallback('" + c.DataField.FieldName + "|" + columnIndex + "|" + rowIndex +"|'+editorText)}";
            else {
                string clientInstanceName = "hidden" + _memberName;
                asPxSpinEdit.ClientSideEvents.ValueChanged = "function(s,e){" + clientInstanceName +
                                                                 ".SetValue(" +
                                                                 asPxSpinEdit.ClientInstanceName + ".GetValue()" + ")}";
            }
            c.Controls.Add(asPxSpinEdit);
        }
        #endregion
    }
}
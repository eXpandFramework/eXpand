using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxHiddenField;
using DevExpress.Web.ASPxPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Web.InPlaceEdit {
    public class CellTemplate : ITemplate
    {
        readonly string _memberName;
        readonly bool _useCallBack;
        readonly ASPxSpinEdit _asPxHiddenField;

        public CellTemplate(string memberName, bool useCallBack,ASPxSpinEdit asPxHiddenField) {
            _memberName = memberName;
            _useCallBack = useCallBack;
            _asPxHiddenField = asPxHiddenField;
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
                if (columnIndex == 0 && rowIndex == 0) {
                    _asPxHiddenField.ClientInstanceName=clientInstanceName;
                    _asPxHiddenField.ID = _asPxHiddenField.ClientInstanceName;
                }
                asPxSpinEdit.ClientSideEvents.ValueChanged = "function(s,e){" + clientInstanceName +
                                                                 ".SetValue(" +
                                                                 asPxSpinEdit.ClientInstanceName + ".GetValue()" + ")}";
            }
            c.Controls.Add(asPxSpinEdit);
        }
        #endregion
    }
}
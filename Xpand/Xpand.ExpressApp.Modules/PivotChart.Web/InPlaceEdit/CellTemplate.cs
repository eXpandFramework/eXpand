using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;

namespace Xpand.ExpressApp.PivotChart.Web.InPlaceEdit {
    public class CellTemplate : ITemplate {
        readonly string _clientInstanceName;

        public CellTemplate(string clientInstanceName) {
            _clientInstanceName = clientInstanceName;
        }
        #region ITemplate Members
        public virtual void InstantiateIn(Control container) {
            var c = (PivotGridCellTemplateContainer) container;
            var asPxSpinEdit = new ASPxSpinEdit {Text = c.Text, Width = Unit.Percentage(100)};
            int columnIndex = ((PivotGridCellTemplateItem) (c.DataItem)).ColumnIndex;
            int rowIndex = ((PivotGridCellTemplateItem) (c.DataItem)).RowIndex;
            asPxSpinEdit.ID = string.Format("Ec{0}r{1}", columnIndex, rowIndex);
            asPxSpinEdit.SpinButtons.ShowIncrementButtons = false;
            asPxSpinEdit.EnableClientSideAPI = true;
            asPxSpinEdit.ClientSideEvents.ValueChanged = "function (s,e){var editorText=s.GetText();" +
                                                         _clientInstanceName + ".PerformCallback('" + columnIndex + "|" +
                                                         rowIndex + "|'+editorText)}";
            asPxSpinEdit.ClientSideEvents.GotFocus = "function(s,e){window.editorToFocus = s.name;ASPxClientControl.GetControlCollection().Get(window.editorToFocus).SelectAll()}";
            c.Controls.Add(asPxSpinEdit);
        }
        #endregion
    }
}
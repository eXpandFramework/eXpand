using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using System.Linq;
using eXpand.ExpressApp.PivotChart.InPlaceEdit;
using eXpand.ExpressApp.PivotChart.Win.Editors;
using eXpand.ExpressApp.PivotChart.Win.PropertyEditors;
using eXpand.Xpo;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase
    {
        readonly Dictionary<PivotGridControl, RepositoryItemSpinEdit>  _repositoryItemSpinEdits = new Dictionary<PivotGridControl, RepositoryItemSpinEdit>();


        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            foreach (var pivotGridControl in GetPivotGridControl())
            {
                var repositoryItemSpinEdit = new RepositoryItemSpinEdit();
                _repositoryItemSpinEdits.Add(pivotGridControl, repositoryItemSpinEdit);
                pivotGridControl.RepositoryItems.Add(repositoryItemSpinEdit);
                pivotGridControl.FieldAreaChanged += _pivotGridControl_FieldAreaChanged;
                pivotGridControl.ShowingEditor += _pivotGridControl_ShowingEditor;
                pivotGridControl.EditValueChanged += _pivotGridControl_EditValueChanged;
            }
        }

        void _pivotGridControl_FieldAreaChanged(object sender, PivotFieldEventArgs e) {
            SetEditor(e.Field,(PivotGridControl) sender);
        }

        void SetEditor(PivotGridField field, PivotGridControl sender) {
            if (field.Area == PivotArea.DataArea && field.DataType.IsPrimitive) {
                field.FieldEdit = _repositoryItemSpinEdits[sender];
            }
        }

        void _pivotGridControl_ShowingEditor(object sender, CancelPivotCellEditEventArgs e) {
            PivotCellEventArgs cellInfo = GetFocusedCellInfo(sender as PivotGridControl);
            if (cellInfo.RowValueType == PivotGridValueType.GrandTotal ||
                cellInfo.ColumnValueType == PivotGridValueType.GrandTotal)
                e.Cancel = true;
        }

        PivotCellEventArgs GetFocusedCellInfo(PivotGridControl pivot) {
            Point focused = pivot.Cells.FocusedCell;
            return pivot.Cells.GetCellInfo(focused.X, focused.Y);
        }

        IEnumerable<PivotGridControl> GetPivotGridControl() {
            return View.GetItems<AnalysisEditorWin>().Select(win => win.Control.PivotGrid).ToArray();
        }

        void _pivotGridControl_EditValueChanged(object sender, EditValueChangedEventArgs e) {
            PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
            for (int j = 0; j < ds.RowCount; j++) {
                ds[j][e.DataField] = XpandReflectionHelper.ChangeType(e.Editor.EditValue, ds[j][e.DataField].GetType());
            }
        }

        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase) {
            var pivotGridControl = (((AnalysisControlWin) analysisEditorBase.Control)).PivotGrid;
            foreach (PivotGridField field in pivotGridControl.Fields) {
                SetEditor(field, pivotGridControl);
            }
        }
    }
}
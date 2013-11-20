using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using Xpand.ExpressApp.PivotChart.InPlaceEdit;
using Xpand.ExpressApp.PivotChart.Win.Editors;
using Xpand.ExpressApp.PivotChart.Win.PropertyEditors;

using Xpand.Xpo;

namespace Xpand.ExpressApp.PivotChart.Win {
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase {
        readonly Dictionary<PivotGridControl, RepositoryItemSpinEdit> _repositoryItemSpinEdits = new Dictionary<PivotGridControl, RepositoryItemSpinEdit>();
        readonly List<PivotGridControl> _pivotGridControls=new List<PivotGridControl>();

        protected override void OnAnalysisEditorCreated(AnalysisEditorBase analysisEditorBase) {
            base.OnAnalysisEditorCreated(analysisEditorBase);
            var analysisEditorWin = ((AnalysisEditorWin)analysisEditorBase);
            var pivotGridControl = analysisEditorWin.Control.PivotGrid;
            _pivotGridControls.Add(pivotGridControl);
            var repositoryItemSpinEdit = new RepositoryItemSpinEdit();
            if (!_repositoryItemSpinEdits.ContainsKey(pivotGridControl))
                _repositoryItemSpinEdits.Add(pivotGridControl, repositoryItemSpinEdit);
            pivotGridControl.RepositoryItems.Add(repositoryItemSpinEdit);
            pivotGridControl.FieldAreaChanged += _pivotGridControl_FieldAreaChanged;
            pivotGridControl.ShowingEditor += _pivotGridControl_ShowingEditor;
            pivotGridControl.CustomCellEditForEditing+=PivotGridControlOnCustomCellEditForEditing;
            pivotGridControl.EditValueChanged += _pivotGridControl_EditValueChanged;
        }

        void PivotGridControlOnCustomCellEditForEditing(object sender, PivotCustomCellEditEventArgs pivotCustomCellEditEventArgs) {
            SetEditor(pivotCustomCellEditEventArgs.DataField,(PivotGridControl) sender);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            foreach (var pivotGridControl in _pivotGridControls) {
                pivotGridControl.ShowingEditor -= _pivotGridControl_ShowingEditor;
                pivotGridControl.CustomCellEditForEditing -= PivotGridControlOnCustomCellEditForEditing;
                pivotGridControl.FieldAreaChanged-=_pivotGridControl_FieldAreaChanged;
                pivotGridControl.EditValueChanged-=_pivotGridControl_EditValueChanged;
            }
        }


        void _pivotGridControl_FieldAreaChanged(object sender, PivotFieldEventArgs e) {
            SetEditor(e.Field, (PivotGridControl)sender);
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

        void _pivotGridControl_EditValueChanged(object sender, EditValueChangedEventArgs e) {
            PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
            for (int j = 0; j < ds.RowCount; j++) {
                ds[j][e.DataField] = XpandReflectionHelper.ChangeType(e.Editor.EditValue, ds[j][e.DataField].GetType());
            }
        }

        protected override void CreateEditors(AnalysisEditorBase analysisEditorBase) {
            var pivotGridControl = (((AnalysisControlWin)analysisEditorBase.Control)).PivotGrid;
            foreach (PivotGridField field in pivotGridControl.Fields) {
                SetEditor(field, pivotGridControl);
            }
        }
    }
}
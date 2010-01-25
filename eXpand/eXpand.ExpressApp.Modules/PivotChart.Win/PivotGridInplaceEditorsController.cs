using System;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class PivotGridInplaceEditorsController : ViewController<DetailView> {
        PivotGridControl _pivotGridControl;
        RepositoryItemSpinEdit _repositoryItemSpinEdit;

        public PivotGridInplaceEditorsController() {
            TargetObjectType = typeof (IAnalysisInfo);
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.ControlsCreated += View_ControlsCreated;
            Frame.GetController<AnalysisDataBindController>().BindDataAction.Execute += BindDataAction_Execute;
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            _pivotGridControl = GetPivotGridControl();
            _repositoryItemSpinEdit = new RepositoryItemSpinEdit();
            _pivotGridControl.RepositoryItems.Add(_repositoryItemSpinEdit);
            _pivotGridControl.FieldAreaChanged += _pivotGridControl_FieldAreaChanged;
            _pivotGridControl.ShowingEditor += _pivotGridControl_ShowingEditor;
            _pivotGridControl.EditValueChanged += _pivotGridControl_EditValueChanged;
        }

        void BindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            foreach (PivotGridField field in _pivotGridControl.Fields) {
                SetEditor(field);
            }
        }

        void _pivotGridControl_FieldAreaChanged(object sender, PivotFieldEventArgs e) {
            SetEditor(e.Field);
        }

        void SetEditor(PivotGridField field) {
            if (field.Area == PivotArea.DataArea && field.DataType.IsPrimitive) {
                field.FieldEdit = _repositoryItemSpinEdit;
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

        PivotGridControl GetPivotGridControl() {
            return ((AnalysisEditorWin) View.FindItem("Self")).Control.PivotGrid;
        }

        void _pivotGridControl_EditValueChanged(object sender, EditValueChangedEventArgs e) {
            PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
            for (int j = 0; j < ds.RowCount; j++) {
                ds[j][e.DataField] = Convert.ToDouble(e.Editor.EditValue);
            }
        }
    }
}
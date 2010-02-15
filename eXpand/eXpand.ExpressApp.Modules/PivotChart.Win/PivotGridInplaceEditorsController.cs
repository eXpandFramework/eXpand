using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class PivotGridInplaceEditorsController : AnalysisViewControllerBase
    {
        readonly Dictionary<PivotGridControl, RepositoryItemSpinEdit>  _repositoryItemSpinEdits = new Dictionary<PivotGridControl, RepositoryItemSpinEdit>();

        public PivotGridInplaceEditorsController() {
            TargetObjectType = typeof (IAnalysisInfo);
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<AnalysisReadOnlyController>().Active[GetType().Name] = false;
            var detailViewInfoNodeWrapper = new DetailViewInfoNodeWrapper(View.Info);
            var itemInfoNodeWrapper= detailViewInfoNodeWrapper.Editors.Items.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.PropertyType)).FirstOrDefault();
            if (itemInfoNodeWrapper!= null&&itemInfoNodeWrapper.AllowEdit){
                View.ControlsCreated += View_ControlsCreated;
                Frame.GetController<AnalysisDataBindController>().BindDataAction.Execute += BindDataAction_Execute;
            }
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            foreach (var pivotGridControl in GetPivotGridControl()) {
                var repositoryItemSpinEdit = new RepositoryItemSpinEdit();
                _repositoryItemSpinEdits.Add(pivotGridControl, repositoryItemSpinEdit);
                pivotGridControl.RepositoryItems.Add(repositoryItemSpinEdit);
                pivotGridControl.FieldAreaChanged += _pivotGridControl_FieldAreaChanged;
                pivotGridControl.ShowingEditor += _pivotGridControl_ShowingEditor;
                pivotGridControl.EditValueChanged += _pivotGridControl_EditValueChanged;
            }
        }

        void BindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            foreach (var pivotGridControl in _repositoryItemSpinEdits.Select(pair => pair.Key)) {
                foreach (PivotGridField field in pivotGridControl.Fields){
                    SetEditor(field, pivotGridControl);
                }    
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
                ds[j][e.DataField] = Convert.ToDouble(e.Editor.EditValue);
            }
        }
    }
}
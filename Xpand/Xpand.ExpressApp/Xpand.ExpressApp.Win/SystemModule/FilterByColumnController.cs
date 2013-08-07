using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class FilterByGridViewColumnController:ViewController<ListView> {
        FilterByColumnController _filterByColumnController;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridView!=null) {
                _filterByColumnController = Frame.GetController<FilterByColumnController>();
                _filterByColumnController.CellFilterAction.Execute+=CellFilterActionOnExecute;
                GridView.FocusedColumnChanged+=GridViewOnFocusedColumnChanged;
            }
        }

        void GridViewOnFocusedColumnChanged(object sender, FocusedColumnChangedEventArgs focusedColumnChangedEventArgs) {
            var columnCellFilter = focusedColumnChangedEventArgs.FocusedColumn.Model() as IModelColumnCellFilter;
            if (columnCellFilter != null) {   
                _filterByColumnController.UpdateAction(columnCellFilter.CellFilter);
            }
        }

        void CellFilterActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            if (GridView != null) {
                var criteria = _filterByColumnController.GetCriteria(View.Model.Columns[GridView.FocusedColumn.Name], GridView.GetFocusedValue(), GridView.ActiveFilterCriteria);
                GridView.ActiveFilterCriteria = criteria;
                GridView.ActiveFilterEnabled = true;
            }
        }

        public GridView GridView {
            get {
                var columnsListEditor = View.Editor as ColumnsListEditor;
                return columnsListEditor != null ? columnsListEditor.GridView() : null;
            }
        }
    }
}

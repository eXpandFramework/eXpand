using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers {
    public class FilterByTreeListColumnController : ViewController<ListView> {
        FilterByColumnController _filterByColumnController;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (TreeList != null) {
                _filterByColumnController = Frame.GetController<FilterByColumnController>();
                _filterByColumnController.CellFilterAction.Execute += CellFilterActionOnExecute;
                TreeList.FocusedColumnChanged += ObjectTreeListOnFocusedColumnChanged;
            }
        }

        void ObjectTreeListOnFocusedColumnChanged(object sender, FocusedColumnChangedEventArgs focusedColumnChangedEventArgs) {
            var treeListColumn = focusedColumnChangedEventArgs.Column;
            if (treeListColumn != null && View != null) {
                var modelColumn = ((IModelColumnCellFilter) View.Model.Columns[treeListColumn.Name]);
                _filterByColumnController.UpdateAction(modelColumn.CellFilter);
            }
        }

        void CellFilterActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            if (TreeList!=null) {
                var modelColumn = View.Model.Columns[TreeList.FocusedColumn.Name];
                var parameters = TreeList.FocusedNode[TreeList.FocusedColumn.Name];
                var activeFilterCriteria = _filterByColumnController.GetCriteria(modelColumn,parameters,TreeList.ActiveFilterCriteria);
                TreeList.ActiveFilterCriteria = activeFilterCriteria;
                TreeList.ActiveFilterEnabled = true;
            }
        }

        public TreeList TreeList {
            get {
                var columnsListEditor = View.Editor as TreeListEditor;
                return columnsListEditor != null ? columnsListEditor.TreeList : null;
            }
        }
    }

}

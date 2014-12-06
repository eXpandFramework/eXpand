using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class ImmediatePostDataController : ViewController<ListView> {
        GridView _gridView;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var gridControl = View.Editor.Control as GridControl;
            if (gridControl != null) {
                _gridView = gridControl.FocusedView as GridView;
                if (_gridView != null) {
                    foreach (GridColumn gridColumn in _gridView.Columns.Where(column => column.ColumnEdit!=null)){
                        var modelColumn = gridColumn.Model();
                        if (modelColumn.ImmediatePostData){
                            gridColumn.ColumnEdit.EditValueChanged+=ColumnEdit_EditValueChanged;
                        }
                    }
                }
            }
        }

        void ColumnEdit_EditValueChanged(object sender, EventArgs e) {
            _gridView.PostEditor();
        }
    }
}

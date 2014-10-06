using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class ImmediatePostDataController : ViewController<ListView> {
        GridView _gridView;

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();            
            var gridControl = View.Editor.Control as GridControl;
            if (gridControl != null){
                _gridView = gridControl.FocusedView as GridView;
                if (_gridView != null){
                    var modelColumns = View.Model.Columns.Where(column => column.ImmediatePostData);
                    foreach (var modelColumn in modelColumns){
                        var gridColumn = _gridView.Columns.ColumnByFieldName(modelColumn.PropertyName);
                        gridColumn.ColumnEdit.EditValueChanged+=ColumnEdit_EditValueChanged;
                    }
                }
            }
        }

        void ColumnEdit_EditValueChanged(object sender, EventArgs e) {
            _gridView.PostEditor();
        }
    }
}
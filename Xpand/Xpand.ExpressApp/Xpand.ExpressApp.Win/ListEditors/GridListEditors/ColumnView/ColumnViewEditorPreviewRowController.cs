using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class ColumnViewEditorPreviewRowController : ListEditorPreviewRowViewController {
        protected override void SetPreviewColumn(IModelColumn previewColumnModel) {
            var listView = (ListView)View;
            if (listView.Editor is IColumnViewEditor) {
                GridControl gridControl = ((IColumnViewEditor)listView.Editor).Grid;
                if (gridControl != null) {
                    var gridView = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (gridView != null) {
                        gridView.PreviewFieldName = previewColumnModel.PropertyName;
                        gridView.OptionsView.ShowPreview = true;
                        gridView.OptionsView.AutoCalcPreviewLineCount = true;
                    }
                }
            }
        }
    }
}
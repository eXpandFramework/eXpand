using DevExpress.ExpressApp.Win.Editors;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public static class Extensions {
        public static DevExpress.XtraGrid.Views.Grid.GridView GridView(this WinColumnsListEditor columnsListEditor) {
            return columnsListEditor.ColumnView as DevExpress.XtraGrid.Views.Grid.GridView;
        }
    }
}

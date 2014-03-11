using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Fasterflect;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public static class EditorExtensions {
        public static string PropertyName(this GridColumn gridColumn) {
            var xafGridColumn = gridColumn as XafGridColumn;
            if (xafGridColumn != null) return (xafGridColumn).PropertyName;
            var xpandGridColumnWrapper = gridColumn as IXafGridColumn;
            if (xpandGridColumnWrapper != null) return xpandGridColumnWrapper.PropertyName;
            return null;
        }

        public static IModelMemberViewItem Model(this GridColumn gridColumn) {
            var xafGridColumn = gridColumn as XafGridColumn;
            if (xafGridColumn != null) return (xafGridColumn).Model;
            var xpandGridColumnWrapper = gridColumn as IXafGridColumn;
            if (xpandGridColumnWrapper != null) return xpandGridColumnWrapper.Model;
            return null;
        }

        public static GridColumn Column(this ColumnWrapper columnWrapper) {
            return columnWrapper.GetPropertyValue("Column") as GridColumn;
        }

        public static RepositoryEditorsFactory RepositoryFactory(this ColumnsListEditor columnsListEditor) {
            var gridListEditor = columnsListEditor as GridListEditor;
            if (gridListEditor != null) return (gridListEditor).RepositoryFactory;
            var columnViewEditor = columnsListEditor as IColumnViewEditor;
            if (columnViewEditor != null) return columnViewEditor.RepositoryFactory;
            throw new NotImplementedException(columnsListEditor.GetType().ToString());
        }

        public static DevExpress.XtraGrid.Views.Grid.GridView GridView(this IColumnViewEditor columnsListEditor) {
            return ((ColumnsListEditor) columnsListEditor).GridView();
        }

        public static DevExpress.XtraGrid.Views.Grid.GridView GridView(this ColumnsListEditor columnsListEditor) {
            var gridListEditor = columnsListEditor as GridListEditor;
            if (gridListEditor != null) return (gridListEditor).GridView;
            var columnViewEditor = columnsListEditor as IColumnViewEditor;
            if (columnViewEditor != null) return columnViewEditor.ColumnView as DevExpress.XtraGrid.Views.Grid.GridView;
            return null;
        }
    }
}

using System.Collections;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public static class MasterDetailColumnViewExtensions {
        public static bool IsDetailView(this IMasterDetailColumnView columnView, IColumnViewEditor editor) {
            return columnView != ((WinColumnsListEditor) editor).Grid.MainView;
        } 
    }
    public interface IMasterDetailColumnView {
        Window Window { get; set; }
        Frame MasterFrame { get; set; }
        GridControl GridControl { get; set; }
        int GetRelationIndex(int sourceRowHandle, string levelName);
    }
    public class CustomGetSelectedObjectsArgs : HandledEventArgs {
        public CustomGetSelectedObjectsArgs(IList list) {
            List = list;
        }

        public IList List { get; set; }
    }
    public class CustomGridCreateEventArgs : HandledEventArgs {
        public GridControl Grid { get; set; }
    }

}

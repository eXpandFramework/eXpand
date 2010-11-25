using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers {
    public class TreeListInplaceEditViewController : ViewController<XpandListView> {
        void treeList_CellValueChanged(object sender, CellValueChangedEventArgs cellValueChangedEventArgs) {
            ReflectionHelper.SetMemberValue(((ObjectTreeList) sender).FocusedObject,
                                            cellValueChangedEventArgs.Column.FieldName, cellValueChangedEventArgs.Value);
            ObjectSpace.CommitChanges();
        }

        void TreeListOnShowingEditor(object sender, CancelEventArgs cancelEventArgs) {
            TreeListColumn treeListColumn = ((TreeList) sender).FocusedColumn;
            cancelEventArgs.Cancel = !View.Model.Columns[treeListColumn.FieldName].AllowEdit;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if ((View.Editor is TreeListEditor) && View.Model.AllowEdit) {
                TreeList treeList = ((TreeListEditor) View.Editor).TreeList;
                foreach (RepositoryItem ri in treeList.RepositoryItems) {
                    ri.ReadOnly = false;
                }
                treeList.CellValueChanged += treeList_CellValueChanged;
                treeList.ShowingEditor += TreeListOnShowingEditor;
                treeList.OptionsBehavior.Editable = true;
            }
        }
    }
}
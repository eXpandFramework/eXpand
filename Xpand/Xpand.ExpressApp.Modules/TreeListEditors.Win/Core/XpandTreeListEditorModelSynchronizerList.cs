using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;

namespace Xpand.ExpressApp.TreeListEditors.Win.Core {
    public class XpandTreeListEditorModelSynchronizerList : TreeListEditorModelSynchronizerList {
        public XpandTreeListEditorModelSynchronizerList(TreeListEditor treeListEditor, IModelListView model)
            : base(treeListEditor, model) {
            Add(new TreeListOptionsModelSynchronizer(treeListEditor.TreeList, model));
            foreach (var modelColumn in model.Columns) {
                Add(new TreeListColumnOptionsModelSynchronizer(treeListEditor.TreeList, modelColumn));
            }
        }
    }
}

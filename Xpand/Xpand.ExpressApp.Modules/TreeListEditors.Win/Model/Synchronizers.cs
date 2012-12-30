using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public TreeListEditorDynamicModelSynchronizer(TreeListEditor editor)
            : base(editor.Control, editor.Model) {
            ModelSynchronizerList.Add(new TreeListViewOptionsSynchronizer(editor.TreeList, ((IModelListViewOptionsTreeList)Model).TreeListOptions));
            ModelSynchronizerList.Add(new TreeListColumnOptionsSynchronizer(editor.TreeList, (IModelListViewOptionsTreeList)Model));
        }

    }
    public class TreeListViewOptionsSynchronizer : TreeListViewOptionsSynchronizer<TreeList> {
        public TreeListViewOptionsSynchronizer(TreeList component, IModelOptionsTreeList modelNode)
            : base(component, modelNode) {
        }
    }

    public class TreeListColumnOptionsSynchronizer : TreeListColumnOptionsSynchronizer<TreeList> {
        public TreeListColumnOptionsSynchronizer(TreeList component, IModelListViewOptionsTreeList modelNode)
            : base(component, modelNode) {
        }


        protected override object Component(IModelColumnOptionsTreeListView column) {
            return Control.Columns[column.PropertyName];
        }
    }
}

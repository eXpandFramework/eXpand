using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
    public class TreeListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public TreeListEditorDynamicModelSynchronizer(ASPxTreeListEditor editor)
            : base(editor.Control, editor.Model) {
            ModelSynchronizerList.Add(new TreeListViewOptionsSynchronizer(editor.TreeList, ((IModelListViewOptionsTreeList)Model).TreeListOptions));
            ModelSynchronizerList.Add(new TreeListColumnOptionsSynchronizer(editor.TreeList, (IModelListViewOptionsTreeList)Model));
        }

    }
    public class TreeListViewOptionsSynchronizer : TreeListViewOptionsSynchronizer<ASPxTreeList> {
        public TreeListViewOptionsSynchronizer(ASPxTreeList component, IModelOptionsTreeList modelNode)
            : base(component, modelNode) {
        }
    }

    public class TreeListColumnOptionsSynchronizer : TreeListColumnOptionsSynchronizer<ASPxTreeList> {
        public TreeListColumnOptionsSynchronizer(ASPxTreeList component, IModelListViewOptionsTreeList modelNode)
            : base(component, modelNode) {
        }


        protected override object Component(IModelColumnOptionsTreeListView column) {
            return Control.Columns[column.PropertyName];
        }
    }

}

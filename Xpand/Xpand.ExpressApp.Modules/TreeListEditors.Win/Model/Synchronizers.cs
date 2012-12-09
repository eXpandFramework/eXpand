using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public TreeListEditorDynamicModelSynchronizer(TreeListEditor editor)
            : base(editor.Control, editor.Model) {
            ModelSynchronizerList.Add(new TreeListViewOptionsSynchronizer(editor.TreeList, ((IModelListViewOptionsTreeList)Model).TreeListOptions));
            ModelSynchronizerList.Add(new TreeListColumnOptionsSynchronizer(editor.TreeList, (IModelListViewOptionsTreeList)Model));
        }

    }
    public class TreeListViewOptionsSynchronizer : ModelSynchronizer<TreeList, IModelOptionsTreeList> {
        public TreeListViewOptionsSynchronizer(TreeList component, IModelOptionsTreeList modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            if (Model.NodeEnabled)
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {

        }
    }

    public class TreeListColumnOptionsSynchronizer : ModelSynchronizer<TreeList, IModelListViewOptionsTreeList> {
        public TreeListColumnOptionsSynchronizer(TreeList component, IModelListViewOptionsTreeList modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            foreach (var column in Model.Columns.OfType<IModelColumnOptionsTreeListView>()) {
                if (column.TreeListColumnOptions.NodeEnabled)
                    ApplyModel(column.TreeListColumnOptions, Control.Columns[column.PropertyName], ApplyValues);
            }
        }

        public override void SynchronizeModel() {

        }
    }
}

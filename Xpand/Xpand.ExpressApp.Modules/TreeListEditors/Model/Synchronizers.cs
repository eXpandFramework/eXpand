using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.ExpressApp.TreeListEditors.Model {

    public abstract class TreeListViewOptionsSynchronizer<TreeList> : ModelSynchronizer<TreeList, IModelOptionsTreeList> {
        protected TreeListViewOptionsSynchronizer(TreeList component, IModelOptionsTreeList modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            if (Model.NodeEnabled)
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {

        }
    }

    public abstract class TreeListColumnOptionsSynchronizer<TreeList> : ModelSynchronizer<TreeList, IModelListViewOptionsTreeList> {
        protected TreeListColumnOptionsSynchronizer(TreeList component, IModelListViewOptionsTreeList modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            foreach (var column in Model.Columns.OfType<IModelColumnOptionsTreeListView>()) {
                if (column.TreeListColumnOptions.NodeEnabled)
                    ApplyModel(column.TreeListColumnOptions, Component(column), ApplyValues);
            }
        }

        protected abstract object Component(IModelColumnOptionsTreeListView column);

        public override void SynchronizeModel() {

        }
    }
}

using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;

namespace Xpand.ExpressApp.TreeListEditors.Win.Core {
    public class XpandTreeListEditorModelSynchronizerList : TreeListEditorModelSynchronizerList {
        private ModelSynchronizerList modelSynchronizerList;
        public XpandTreeListEditorModelSynchronizerList(TreeListEditor treeListEditor, IModelListView model)
            : base(treeListEditor, model) {
            modelSynchronizerList = new ModelSynchronizerList();
            modelSynchronizerList.Add(new TreeListOptionsModelSynchronizer(treeListEditor.TreeList, model));
            foreach (var modelColumn in model.Columns) {
                modelSynchronizerList.Add(new TreeListColumnOptionsModelSynchronizer(treeListEditor.TreeList, modelColumn));
            }
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            modelSynchronizerList.ApplyModel();
        }

        public override void SynchronizeModel() {
            base.SynchronizeModel();
            modelSynchronizerList.SynchronizeModel();
        }
    }
}

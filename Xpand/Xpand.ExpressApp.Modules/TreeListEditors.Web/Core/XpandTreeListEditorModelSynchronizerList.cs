using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Web;

namespace Xpand.ExpressApp.TreeListEditors.Web.Core {
    public class XpandTreeListEditorModelSynchronizerList : ASPxTreeListEditorModelSynchronizer {
        private readonly ModelSynchronizerList modelSynchronizerList;
        public XpandTreeListEditorModelSynchronizerList(ASPxTreeListEditor treeListEditor, IModelListView model)
            : base(treeListEditor, model) {
            modelSynchronizerList = new ModelSynchronizerList
                                    {new TreeListOptionsModelSynchronizer<IModelListView>(treeListEditor.TreeList, model)};
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

using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base.General;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Win.ListEditors {
    [ListEditor(typeof(ITreeNode), true)]
    public class XpandTreeListEditor : DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditor {
        public XpandTreeListEditor(DevExpress.ExpressApp.Model.IModelListView model)
            : base(model) {
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new XpandTreeListEditorModelSynchronizerList(this, Model);
        }
    }

}

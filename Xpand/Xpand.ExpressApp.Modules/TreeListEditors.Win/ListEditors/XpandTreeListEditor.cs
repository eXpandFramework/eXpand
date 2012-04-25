using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Win.ListEditors {
    [ListEditor(typeof(ITreeNode), true)]
    public class XpandTreeListEditor : DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditor, ITreeListEditor {


        public XpandTreeListEditor(IModelListView model)
            : base(model) {
        }


        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new XpandTreeListEditorModelSynchronizerList(this, Model);
        }




    }

}

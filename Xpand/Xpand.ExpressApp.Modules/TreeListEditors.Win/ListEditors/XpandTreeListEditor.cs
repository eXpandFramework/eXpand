using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;

namespace Xpand.ExpressApp.TreeListEditors.Win.ListEditors {
    [ListEditor(typeof(ITreeNode), true)]
    public class XpandTreeListEditor : DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditor {
        public XpandTreeListEditor(IModelListView model)
            : base(model) {
        }
    }

}

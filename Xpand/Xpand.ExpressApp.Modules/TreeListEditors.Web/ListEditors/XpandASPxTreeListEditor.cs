using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Web;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Web.ListEditors {
    public class XpandASPxTreeListEditor : ASPxTreeListEditor, ITreeListEditor {
        public XpandASPxTreeListEditor(IModelListView model)
            : base(model) {
        }

    }
}

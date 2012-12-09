using Common.Win.GridView;
using Common.Win.GridView.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;

namespace XVideoRental.Module.Win.Editors.Common {
    [ListEditor(typeof(object), true)]
    public class XGridListEditor : XpandGridListEditor {
        public XGridListEditor(IModelListView model)
            : base(model) {
        }
    }
    public class XGridViewModelAdapterController : GridViewModelAdapterController {

    }

}

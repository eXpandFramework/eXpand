using Common.Win.LayoutView;
using Common.Win.LayoutView.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model;

namespace XVideoRental.Module.Win.Editors.Common {
    [ListEditor(typeof(object), false)]
    public class XLayoutListEditor : LayoutViewListEditor {
        public XLayoutListEditor(IModelListView model)
            : base(model) {
        }
    }

    public class XLayoutViewModelAdapterController : LayoutViewModelAdapterController {

    }

}


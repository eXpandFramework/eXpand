using Common.Win.AdvBandedView;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.Editors.Common {
    [ListEditor(typeof(object), false)]
    public class XAdvBandedListEditor : AdvBandedListEditor {
        public XAdvBandedListEditor(IModelListView model)
            : base(model) {
        }
    }
    public class AdvBandedViewModelAdapterController : global::Common.Win.AdvBandedView.Model.AdvBandedViewModelAdapterController {

    }

}

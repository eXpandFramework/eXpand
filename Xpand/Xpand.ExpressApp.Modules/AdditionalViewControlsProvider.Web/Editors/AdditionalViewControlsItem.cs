using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Editors {

    [DetailViewItem(typeof(IModelAdditionalViewControlsItem))]
    public class AdditionalViewControlsItem : AdditionalViewControlsProvider.Editors.AdditionalViewControlsItem {

        public AdditionalViewControlsItem(Type objectType, string id)
            : base(objectType, id) {
        }

        public AdditionalViewControlsItem(IModelAdditionalViewControlsItem model, Type objectType)
            : base(model, objectType) {
        }

        protected override object CreateControlCore() {
            return new Panel();
        }
    }
}

using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Editors;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Editors
{
    [DetailViewItem(typeof(IModelAdditionalViewControlsItem))]
    public class AdditionalViewControlsItem : AdditionalViewControlsProvider.Editors.AdditionalViewControlsItem
    {

        public AdditionalViewControlsItem(Type objectType, string id) : base(objectType, id) {
        }

        public AdditionalViewControlsItem(IModelAdditionalViewControlsItem model, Type objectType) : base(model, objectType) {
        }

        protected override object CreateControlCore() {
            var panel = new Panel();
            return panel;
        }
    }
}

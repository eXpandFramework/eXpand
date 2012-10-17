using DevExpress.ExpressApp.Model;
using DevExpress.LookAndFeel;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Design {
    internal class AdvBandedViewPropertyEditor : ColumnViewPropertyEditor<AdvBandedListEditor> {

        protected override ColumnViewDesignerForm GetViewDesignerForm(IModelListView listView) {
            return new AdvBandedViewDesignerForm(UserLookAndFeel.Default, listView.Application.Title);
        }

        protected override AdvBandedListEditor GetGridDesignerEditor(IModelListView listView) {
            return new AdvBandedListEditor(listView);
        }
    }
}
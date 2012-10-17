using DevExpress.ExpressApp.Model;
using DevExpress.LookAndFeel;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Design {
    public class LayoutViewPropertyEditor : ColumnViewPropertyEditor<LayoutViewListEditor> {
        protected override ColumnViewDesignerForm GetViewDesignerForm(IModelListView listView) {
            return new LayoutViewDesignerForm(UserLookAndFeel.Default, listView.Application.Title);
        }

        protected override LayoutViewListEditor GetGridDesignerEditor(IModelListView listView) {
            return new LayoutViewListEditor(listView);
        }
    }
}
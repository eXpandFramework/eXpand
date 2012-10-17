using DevExpress.ExpressApp.Model;
using DevExpress.LookAndFeel;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Design {
    internal class GridViewPropertyEditor : ColumnViewPropertyEditor<XpandGridListEditor> {
        protected override ColumnViewDesignerForm GetViewDesignerForm(IModelListView listView) {
            return new GridViewDesignerForm(UserLookAndFeel.Default, listView.Application.Title);
        }

        protected override XpandGridListEditor GetGridDesignerEditor(IModelListView listView) {
            return new XpandGridListEditor(listView);
        }
    }
}
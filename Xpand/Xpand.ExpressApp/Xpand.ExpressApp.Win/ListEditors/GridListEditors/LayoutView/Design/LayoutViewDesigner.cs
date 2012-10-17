using DevExpress.XtraGrid.Views.Layout.Designer;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Design {
    public class LayoutViewDesigner : ColumnViewDesigner {
        protected override void CreateGroups() {
            base.CreateGroups();
            DefaultGroup.Add("Layout", "Customize the current view's layout and preview its data.", typeof(LayoutViewCustomizer), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
        }
    }
}

using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Design {
    public class LayoutViewDesignerForm : ColumnViewDesignerForm {
        public LayoutViewDesignerForm(UserLookAndFeel userLookAndFeel, string title)
            : base(userLookAndFeel, title) {
        }

        protected override BaseDesigner GetActiveDesigner() {
            return new LayoutViewDesigner();
        }
    }
}
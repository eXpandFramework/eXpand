using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Design {
    internal class GridViewDesignerForm : ColumnViewDesignerForm {
        public GridViewDesignerForm(UserLookAndFeel userLookAndFeel, string title)
            : base(userLookAndFeel, title) {
        }

        protected override BaseDesigner GetActiveDesigner() {
            return new GridViewDesigner();
        }
    }
}
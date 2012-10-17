using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Design {
    internal class AdvBandedViewDesignerForm : ColumnViewDesignerForm {
        public AdvBandedViewDesignerForm(UserLookAndFeel userLookAndFeel, string title)
            : base(userLookAndFeel, title) {
        }

        protected override BaseDesigner GetActiveDesigner() {
            return new AdvBandedViewDesigner();
        }
    }
}
using System;
using DevExpress.ExpressApp.TreeListEditors.Win;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListEditorVisibilityCalculatorHelper : TreeListEditors.Model.TreeListEditorVisibilityCalculatorHelper {
        public override Type TreelistEditorType() {
            return typeof(TreeListEditor);
        }
    }
}

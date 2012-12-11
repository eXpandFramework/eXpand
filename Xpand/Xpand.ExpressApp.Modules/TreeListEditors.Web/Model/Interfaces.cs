using System;
using DevExpress.ExpressApp.TreeListEditors.Web;

namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
    public class TreeListEditorVisibilityCalculatorHelper : TreeListEditors.Model.TreeListEditorVisibilityCalculatorHelper {
        public override Type TreelistEditorType() {
            return typeof(ASPxTreeListEditor);
        }
    }

}

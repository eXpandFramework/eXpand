using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridListEditorVisibilityCalculatorHelper : ExpressApp.Model.Options.GridListEditorVisibilityCalculatorHelper {
        public override bool IsVisible(IModelNode node, string propertyName) {
            Type editorType = EditorType(node);
            if (editorType == typeof(ASPxGridListEditor))
                return true;
            if (typeof(XpandASPxGridListEditor).IsAssignableFrom(editorType))
                return true;
            return false;
        }

    }

}

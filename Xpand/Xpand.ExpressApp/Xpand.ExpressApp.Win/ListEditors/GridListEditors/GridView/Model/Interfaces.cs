using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    public class GridListEditorVisibilityCalculatorHelper : ExpressApp.Model.Options.GridListEditorVisibilityCalculatorHelper {
        public override bool IsVisible(IModelNode node, string propertyName) {
            Type editorType = EditorType(node);
            if (editorType == typeof(GridListEditor))
                return true;
            if (typeof(XpandGridListEditor).IsAssignableFrom(editorType) && !typeof(AdvBandedListEditor).IsAssignableFrom(editorType))
                return true;
            return false;
        }

    }
}

using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    [ModelPersistentName("GridViewOptions")]
    public interface IModelOptionsGridView : IModelOptionsColumnView {
    }
    public interface IModelOptionsColumnGridView : IModelColumnViewColumnOptions {
    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsGridView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsColumnGridView OptionsColumnGridView { get; }
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsGridView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsGridView GridViewOptions { get; }

    }
    public class GridListEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            Type editorType = EditorType(node);
            if (editorType == typeof(GridListEditor))
                return true;
            if (typeof(XpandGridListEditor).IsAssignableFrom(editorType) && !typeof(AdvBandedListEditor).IsAssignableFrom(editorType))
                return true;
            return false;
        }
        #endregion
    }
}

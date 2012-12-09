using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    [ModelAbstractClass]
    public interface IModelListViewOptionsTreeList : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(TreeListEditorVisibilityCalculator))]
        IModelOptionsTreeList TreeListOptions { get; }
    }

    public interface IModelOptionsTreeList : IModelOptionsColumnView {

    }
    public class TreeListEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(TreeListEditor).IsAssignableFrom(EditorType(node));
        }
        #endregion
    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsTreeListView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(TreeListEditorVisibilityCalculator))]
        IModelOptionsColumnTreeListView TreeListColumnOptions { get; }
    }
    public interface IModelOptionsColumnTreeListView : IModelColumnViewColumnOptions {
    }

}

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Design;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model {
    public interface IModelOptionsLayoutView : IModelOptionsColumnView {
        IModelDesignLayoutView DesignLayoutView { get; }
    }

    public interface IModelDesignLayoutView : IModelLayoutDesignStore {
        [Editor(typeof(LayoutViewPropertyEditor), typeof(UITypeEditor))]
        [DefaultValue("Use the property editor to invoke the designer")]
        string Layout { get; set; }
    }

    public interface IModelOptionsColumnLayoutView : IModelColumnViewColumnOptions {
    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsLayoutView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(LayoutEditorVisibilityCalculator))]
        IModelOptionsColumnLayoutView OptionsColumnLayoutView { get; }
    }
    [ModelAbstractClass]
    public interface IModelListViewOptionsLayoutView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(LayoutEditorVisibilityCalculator))]
        IModelOptionsLayoutView OptionsLayoutView { get; }
    }
    public class LayoutEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(LayoutViewListEditor).IsAssignableFrom(EditorType(node));
        }
        #endregion
    }

}

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Design;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model {
    public interface IModelLayoutViewDesign : IModelNode {
        IModelLayoutDesignStore DesignLayoutView { get; }
    }

    public interface IModelLayoutDesignStore :IModelNode {
        [Editor(typeof(LayoutViewPropertyEditor), typeof(UITypeEditor))]
        [DefaultValue("Use the property editor to invoke the designer")]
        string Layout { get; set; }
        [Browsable(false)]
        string LayoutStore { get; set; }
    }

//    public interface IModelOptionsColumnLayoutView : IModelColumnViewColumnOptions {
//    }

//    [ModelAbstractClass]
//    public interface IModelColumnOptionsLayoutView : IModelColumnOptionsColumnView {
//        [ModelBrowsable(typeof(LayoutEditorVisibilityCalculator))]
//        IModelOptionsColumnLayoutView OptionsColumnLayoutView { get; }
//    }
//    [ModelAbstractClass]
//    public interface IModelListViewOptionsLayoutView : IModelListViewOptionsColumnView {
//        [ModelBrowsable(typeof(LayoutEditorVisibilityCalculator))]
//        IModelOptionsLayoutView OptionsLayoutView { get; }
//    }
//    public class LayoutEditorVisibilityCalculator : EditorTypeVisibilityCalculator<LayoutViewListEditor,IModelListView> {
//    }

}

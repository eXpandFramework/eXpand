using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model {

    public interface IModelGridBands : IModelNode, IModelList<IModelGridBand> {
    }

    public interface IModelGridBand : IModelNode {
        IModelGridBands GridBands { get; }
    }

    public interface IModelOptionsAdvBandedView : IModelOptionsColumnView {
        IModelGridBands GridBands { get; }
    }

    public interface IModelOptionsColumnAdvBandedView : IModelColumnViewColumnOptions {
    }

    public interface IModelAdvBandedGridBandOptions : IModelNode {
    }


    [ModelAbstractClass]
    public interface IModelColumnOptionsAdvBandedView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelOptionsColumnAdvBandedView OptionsColumnAdvBandedView { get; }
        [DataSourceProperty("ListViewBands")]
        [Category("eXpand")]
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelGridBand GridBand { get; set; }

        [Browsable(false)]
        IModelList<IModelGridBand> ListViewBands { get; }
    }

    [DomainLogic(typeof(IModelColumnOptionsAdvBandedView))]
    public class MasterDetailRuleDomainLogic {
        public static IModelList<IModelGridBand> Get_ListViewBands(IModelColumnOptionsAdvBandedView modelColumnOptionsAdvBandedView) {
            var viewBands = ((IModelListViewOptionsAdvBandedView)modelColumnOptionsAdvBandedView.ParentView).OptionsAdvBandedView.GridBands;
            return new CalculatedModelNodeList<IModelGridBand>(viewBands.GetItems<IModelGridBand>(band => band.GridBands));
        }
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsAdvBandedView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelOptionsAdvBandedView OptionsAdvBandedView { get; }
    }

    public class AdvBandedEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(AdvBandedListEditor).IsAssignableFrom(EditorType(node));
        }
        #endregion
    }

    public interface IModelAdvBandedViewDesign : IModelLayoutDesignStore {
        [Editor(typeof(AdvBandedViewPropertyEditor), typeof(UITypeEditor))]
        [DefaultValue("Use the property editor to invoke the designer")]
        string Layout { get; set; }
    }
}

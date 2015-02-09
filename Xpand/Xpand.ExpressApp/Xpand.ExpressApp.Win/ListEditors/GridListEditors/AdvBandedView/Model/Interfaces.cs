using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model {
    public interface IModelGridBand : IModelNode {
        IModelGridBands GridBands { get; }
    }

    public interface IModelGridBands : IModelNode, IModelList<IModelGridBand> {
    }

    [ModelDisplayName("AdvBandedViewOptions")]
    public interface IModelOptionsAdvBandedView : IModelOptionsColumnView {
        [Browsable(false)]
        IModelGridBands GridBands { get; }
    }

    public interface IModelOptionsColumnAdvBandedView : IModelColumnViewColumnOptions {
    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsAdvBandedView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelOptionsColumnAdvBandedView OptionsColumnAdvBandedView { get; }
        [DataSourceProperty("ListViewBands")]
        [Category("eXpand")]
        [Browsable(false)]
        IModelGridBand GridBand { get; set; }

        [Browsable(false)]
        IModelList<IModelGridBand> ListViewBands { get; }
    }

    [DomainLogic(typeof(IModelColumnOptionsAdvBandedView))]
    public class ModelColumnOptionsAdvBandedViewDomainLogic {
        public static IModelList<IModelGridBand> Get_ListViewBands(IModelColumnOptionsAdvBandedView modelColumnOptionsAdvBandedView) {
            var viewBands = ((IModelListViewOptionsAdvBandedView)modelColumnOptionsAdvBandedView.ParentView).OptionsAdvBandedView.GridBands;
            return new CalculatedModelNodeList<IModelGridBand>(viewBands.GetItems<IModelGridBand>(band => band.GridBands));
        }
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsAdvBandedView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelOptionsAdvBandedView OptionsAdvBandedView { get; }
        [ModelBrowsable(typeof(AdvBandedEditorVisibilityCalculator))]
        IModelAdvBandedViewModelAdapters AdvBandedViewModelAdapters { get; }
    }

    public class AdvBandedEditorVisibilityCalculator : EditorTypeVisibilityCalculator<GridListEditor, IModelListView> {
        public override bool IsVisible(IModelNode node, string propertyName){
            return base.IsVisible(node, propertyName)&&node.GetParent<IModelListView>().BandsLayout.Enable;
        }
    }

    [ModelNodesGenerator(typeof(ModelAdvBandedViewAdaptersNodeGenerator))]
    public interface IModelAdvBandedViewModelAdapters : IModelList<IModelAdvBandedViewModelAdapter>, IModelNode {

    }

    public interface IModelAdvBandedViewModelAdapter : IModelCommonModelAdapter<IModelOptionsAdvBandedView> {

    }

    [DomainLogic(typeof(IModelAdvBandedViewModelAdapter))]
    public class ModelAdvBandedViewModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelOptionsAdvBandedView> {
        public static IModelList<IModelOptionsAdvBandedView> Get_ModelAdapters(IModelAdvBandedViewModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public class ModelAdvBandedViewAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelOptionsAdvBandedView,IModelAdvBandedViewModelAdapter> {

    }

}

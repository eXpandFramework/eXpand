using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ZoomTrackBar")]
    public interface IModelRepositoryItemZoomTrackBar : IModelRepositoryItem {
        IModelRepositoryItemZoomTrackBarModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemZoomTrackBarAdaptersNodeGenerator))]
    public interface IModelRepositoryItemZoomTrackBarModelAdapters : IModelList<IModelRepositoryItemZoomTrackBarModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemZoomTrackBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemZoomTrackBar, IModelRepositoryItemZoomTrackBarModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemZoomTrackBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemZoomTrackBar> {
    }

    [DomainLogic(typeof(IModelRepositoryItemZoomTrackBarModelAdapter))]
    public class ModelRepositoryItemZoomTrackBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemZoomTrackBar> {
        public static IModelList<IModelRepositoryItemZoomTrackBar> Get_ModelAdapters(IModelRepositoryItemZoomTrackBarModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
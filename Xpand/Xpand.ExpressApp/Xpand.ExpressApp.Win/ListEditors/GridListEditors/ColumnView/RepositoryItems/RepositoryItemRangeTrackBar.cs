using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("RangeTrackBar")]
    public interface IModelRepositoryItemRangeTrackBar : IModelRepositoryItem {
        IModelRepositoryItemRangeTrackBarModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemRangeTrackBarAdaptersNodeGenerator))]
    public interface IModelRepositoryItemRangeTrackBarModelAdapters : IModelList<IModelRepositoryItemRangeTrackBarModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemRangeTrackBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemRangeTrackBar, IModelRepositoryItemRangeTrackBarModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemRangeTrackBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemRangeTrackBar> {
    }

    [DomainLogic(typeof(IModelRepositoryItemRangeTrackBarModelAdapter))]
    public class ModelRepositoryItemRangeTrackBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemRangeTrackBar> {
        public static IModelList<IModelRepositoryItemRangeTrackBar> Get_ModelAdapters(IModelRepositoryItemRangeTrackBarModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
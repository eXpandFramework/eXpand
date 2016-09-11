using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("TrackBar")]
    [RepositoryItem(typeof(RepositoryItemTrackBar))]
    public interface IModelRepositoryItemTrackBar : IModelRepositoryItem {
        IModelRepositoryItemTrackBarModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemTrackBarAdaptersNodeGenerator))]
    public interface IModelRepositoryItemTrackBarModelAdapters : IModelList<IModelRepositoryItemTrackBarModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemTrackBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemTrackBar, IModelRepositoryItemTrackBarModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemTrackBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemTrackBar> {
    }

    [DomainLogic(typeof(IModelRepositoryItemTrackBarModelAdapter))]
    public class ModelRepositoryItemTrackBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemTrackBar> {
        public static IModelList<IModelRepositoryItemTrackBar> Get_ModelAdapters(IModelRepositoryItemTrackBarModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
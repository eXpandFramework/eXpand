using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("BlobBaseEdit")]
    public interface IModelRepositoryItemBlobBaseEdit : IModelRepositoryItem {
        IModelRepositoryItemBlobBaseEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemBlobBaseEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemBlobBaseEditModelAdapters : IModelList<IModelRepositoryItemBlobBaseEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemBlobBaseEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemBlobBaseEdit, IModelRepositoryItemBlobBaseEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemBlobBaseEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemBlobBaseEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemBlobBaseEditModelAdapter))]
    public class ModelRepositoryItemBlobBaseEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemBlobBaseEdit> {
        public static IModelList<IModelRepositoryItemBlobBaseEdit> Get_ModelAdapters(IModelRepositoryItemBlobBaseEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
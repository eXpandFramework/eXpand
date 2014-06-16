using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ImageEdit")]
    public interface IModelRepositoryItemImageEdit : IModelRepositoryItem {
        IModelRepositoryItemImageEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemImageEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemImageEditModelAdapters : IModelList<IModelRepositoryItemImageEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemImageEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemImageEdit, IModelRepositoryItemImageEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemImageEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemImageEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemImageEditModelAdapter))]
    public class ModelRepositoryItemImageEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemImageEdit> {
        public static IModelList<IModelRepositoryItemImageEdit> Get_ModelAdapters(IModelRepositoryItemImageEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
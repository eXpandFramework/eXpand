using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("MemoExEdit")]
    public interface IModelRepositoryItemMemoExEdit : IModelRepositoryItem {
        IModelRepositoryItemMemoExEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemMemoExEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemMemoExEditModelAdapters : IModelList<IModelRepositoryItemMemoExEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemMemoExEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemMemoExEdit, IModelRepositoryItemMemoExEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemMemoExEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemMemoExEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemMemoExEditModelAdapter))]
    public class ModelRepositoryItemMemoExEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemMemoExEdit> {
        public static IModelList<IModelRepositoryItemMemoExEdit> Get_ModelAdapters(IModelRepositoryItemMemoExEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
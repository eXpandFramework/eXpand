using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("MemoEdit")]
    public interface IModelRepositoryItemMemoEdit : IModelRepositoryItem {
        IModelRepositoryItemMemoEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemMemoEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemMemoEditModelAdapters : IModelList<IModelRepositoryItemMemoEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemMemoEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemMemoEdit, IModelRepositoryItemMemoEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemMemoEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemMemoEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemMemoEditModelAdapter))]
    public class ModelRepositoryItemMemoEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemMemoEdit> {
        public static IModelList<IModelRepositoryItemMemoEdit> Get_ModelAdapters(IModelRepositoryItemMemoEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
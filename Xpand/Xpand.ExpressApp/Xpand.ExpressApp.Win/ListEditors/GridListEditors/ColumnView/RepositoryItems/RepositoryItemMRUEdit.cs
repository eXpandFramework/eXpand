using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("MRUEdit")]
    [RepositoryItem(typeof(RepositoryItemMRUEdit))]
    public interface IModelRepositoryItemMRUEdit : IModelRepositoryItem {
        IModelRepositoryItemMRUEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemMRUEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemMRUEditModelAdapters : IModelList<IModelRepositoryItemMRUEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemMRUEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemMRUEdit, IModelRepositoryItemMRUEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemMRUEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemMRUEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemMRUEditModelAdapter))]
    public class ModelRepositoryItemMRUEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemMRUEdit> {
        public static IModelList<IModelRepositoryItemMRUEdit> Get_ModelAdapters(IModelRepositoryItemMRUEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
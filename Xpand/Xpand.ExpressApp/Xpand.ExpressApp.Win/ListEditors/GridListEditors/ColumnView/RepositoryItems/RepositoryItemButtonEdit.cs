using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ButtonEdit")]
    [RepositoryItem(typeof(RepositoryItemButtonEdit))]
    public interface IModelRepositoryItemButtonEdit : IModelRepositoryItem {
        IModelRepositoryItemButtonEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemButtonEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemButtonEditModelAdapters : IModelList<IModelRepositoryItemButtonEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemButtonEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemButtonEdit, IModelRepositoryItemButtonEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemButtonEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemButtonEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemButtonEditModelAdapter))]
    public class ModelRepositoryItemButtonEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemButtonEdit> {
        public static IModelList<IModelRepositoryItemButtonEdit> Get_ModelAdapters(IModelRepositoryItemButtonEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
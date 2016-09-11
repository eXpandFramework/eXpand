using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("LookUpEdit")]
    [RepositoryItem(typeof(RepositoryItemLookUpEdit))]
    public interface IModelRepositoryItemLookUpEdit : IModelRepositoryItem {
        IModelRepositoryItemLookupEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemLookupEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemLookupEditModelAdapters : IModelList<IModelRepositoryItemLookupEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemLookupEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemLookUpEdit, IModelRepositoryItemLookupEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemLookupEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemLookUpEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemLookupEditModelAdapter))]
    public class ModelRepositoryItemLookupEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemLookUpEdit> {
        public static IModelList<IModelRepositoryItemLookUpEdit> Get_ModelAdapters(IModelRepositoryItemLookupEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
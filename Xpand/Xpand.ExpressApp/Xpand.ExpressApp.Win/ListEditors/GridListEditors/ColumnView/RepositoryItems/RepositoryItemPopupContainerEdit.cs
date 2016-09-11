using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PopupContainerEdit")]
    [RepositoryItem(typeof(RepositoryItemPopupContainerEdit))]
    public interface IModelRepositoryItemPopupContainerEdit : IModelRepositoryItem {
        IModelRepositoryItemPopupContainerEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPopupContainerEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPopupContainerEditModelAdapters : IModelList<IModelRepositoryItemPopupContainerEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPopupContainerEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPopupContainerEdit, IModelRepositoryItemPopupContainerEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPopupContainerEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPopupContainerEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPopupContainerEditModelAdapter))]
    public class ModelRepositoryItemPopupContainerEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPopupContainerEdit> {
        public static IModelList<IModelRepositoryItemPopupContainerEdit> Get_ModelAdapters(IModelRepositoryItemPopupContainerEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
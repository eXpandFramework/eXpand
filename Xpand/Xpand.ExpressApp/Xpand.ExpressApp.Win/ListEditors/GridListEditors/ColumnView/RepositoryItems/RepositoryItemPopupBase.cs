using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PopupBase")]
    [RepositoryItem(typeof(RepositoryItemPopupBase))]
    public interface IModelRepositoryItemPopupBase : IModelRepositoryItem {
        IModelRepositoryItemPopupBaseModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPopupBaseAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPopupBaseModelAdapters : IModelList<IModelRepositoryItemPopupBaseModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPopupBaseAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPopupBase, IModelRepositoryItemPopupBaseModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPopupBaseModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPopupBase> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPopupBaseModelAdapter))]
    public class ModelRepositoryItemPopupBaseModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPopupBase> {
        public static IModelList<IModelRepositoryItemPopupBase> Get_ModelAdapters(IModelRepositoryItemPopupBaseModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
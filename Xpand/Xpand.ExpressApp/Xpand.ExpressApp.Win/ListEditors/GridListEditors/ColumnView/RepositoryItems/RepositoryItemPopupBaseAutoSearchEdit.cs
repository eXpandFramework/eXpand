using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PopupBaseAutoSearchEdit")]
    public interface IModelRepositoryItemPopupBaseAutoSearchEdit : IModelRepositoryItem {
        IModelRepositoryItemPopupBaseAutoSearchEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPopupBaseAutoSearchEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPopupBaseAutoSearchEditModelAdapters : IModelList<IModelRepositoryItemPopupBaseAutoSearchEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPopupBaseAutoSearchEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPopupBaseAutoSearchEdit, IModelRepositoryItemPopupBaseAutoSearchEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPopupBaseAutoSearchEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPopupBaseAutoSearchEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPopupBaseAutoSearchEditModelAdapter))]
    public class ModelRepositoryItemPopupBaseAutoSearchEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPopupBaseAutoSearchEdit> {
        public static IModelList<IModelRepositoryItemPopupBaseAutoSearchEdit> Get_ModelAdapters(IModelRepositoryItemPopupBaseAutoSearchEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PopupCriteriaEdit")]
    public interface IModelRepositoryItemPopupCriteriaEdit : IModelRepositoryItem {
        IModelRepositoryItemPopupCriteriaEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPopupCriteriaEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPopupCriteriaEditModelAdapters : IModelList<IModelRepositoryItemPopupCriteriaEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPopupCriteriaEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPopupCriteriaEdit, IModelRepositoryItemPopupCriteriaEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPopupCriteriaEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPopupCriteriaEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPopupCriteriaEditModelAdapter))]
    public class ModelRepositoryItemPopupCriteriaEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPopupCriteriaEdit> {
        public static IModelList<IModelRepositoryItemPopupCriteriaEdit> Get_ModelAdapters(IModelRepositoryItemPopupCriteriaEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PopupExpressionEdit")]
    public interface IModelRepositoryItemPopupExpressionEdit : IModelRepositoryItem {
        IModelRepositoryItemPopupExpressionEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPopupExpressionEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPopupExpressionEditModelAdapters : IModelList<IModelRepositoryItemPopupExpressionEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPopupExpressionEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPopupExpressionEdit, IModelRepositoryItemPopupExpressionEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPopupExpressionEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPopupExpressionEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPopupExpressionEditModelAdapter))]
    public class ModelRepositoryItemPopupExpressionEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPopupExpressionEdit> {
        public static IModelList<IModelRepositoryItemPopupExpressionEdit> Get_ModelAdapters(IModelRepositoryItemPopupExpressionEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
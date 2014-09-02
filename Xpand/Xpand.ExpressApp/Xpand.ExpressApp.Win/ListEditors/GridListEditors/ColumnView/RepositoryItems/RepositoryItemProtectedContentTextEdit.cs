using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ProtectedContentTextEdit")]
    public interface IModelRepositoryItemProtectedContentTextEdit : IModelRepositoryItem {
        IModelRepositoryItemProtectedContentTextEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemProtectedContentTextEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemProtectedContentTextEditModelAdapters : IModelList<IModelRepositoryItemProtectedContentTextEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemProtectedContentTextEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemProtectedContentTextEdit, IModelRepositoryItemProtectedContentTextEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemProtectedContentTextEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemProtectedContentTextEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemProtectedContentTextEditModelAdapter))]
    public class ModelRepositoryItemProtectedContentTextEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemProtectedContentTextEdit> {
        public static IModelList<IModelRepositoryItemProtectedContentTextEdit> Get_ModelAdapters(IModelRepositoryItemProtectedContentTextEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
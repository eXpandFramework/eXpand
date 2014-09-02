using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("TimeEdit")]
    public interface IModelRepositoryItemTimeEdit : IModelRepositoryItem {
        IModelRepositoryItemTimeEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemTimeEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemTimeEditModelAdapters : IModelList<IModelRepositoryItemTimeEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemTimeEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemTimeEdit, IModelRepositoryItemTimeEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemTimeEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemTimeEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemTimeEditModelAdapter))]
    public class ModelRepositoryItemTimeEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemTimeEdit> {
        public static IModelList<IModelRepositoryItemTimeEdit> Get_ModelAdapters(IModelRepositoryItemTimeEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
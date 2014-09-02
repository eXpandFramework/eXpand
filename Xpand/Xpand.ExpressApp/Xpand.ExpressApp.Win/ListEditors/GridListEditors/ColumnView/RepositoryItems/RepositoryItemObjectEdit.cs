using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ObjectEdit")]
    public interface IModelRepositoryItemObjectEdit : IModelRepositoryItem {
        IModelRepositoryItemObjectEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemObjectEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemObjectEditModelAdapters : IModelList<IModelRepositoryItemObjectEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemObjectEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemObjectEdit, IModelRepositoryItemObjectEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemObjectEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemObjectEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemObjectEditModelAdapter))]
    public class ModelRepositoryItemObjectEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemObjectEdit> {
        public static IModelList<IModelRepositoryItemObjectEdit> Get_ModelAdapters(IModelRepositoryItemObjectEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
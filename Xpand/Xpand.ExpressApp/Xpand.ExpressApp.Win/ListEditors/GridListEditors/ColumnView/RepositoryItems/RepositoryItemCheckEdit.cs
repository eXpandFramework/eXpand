using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("CheckEdit")]
    public interface IModelRepositoryItemCheckEdit : IModelRepositoryItem {
        IModelRepositoryItemCheckEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemCheckEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemCheckEditModelAdapters : IModelList<IModelRepositoryItemCheckEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemCheckEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemCheckEdit, IModelRepositoryItemCheckEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemCheckEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemCheckEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemCheckEditModelAdapter))]
    public class ModelRepositoryItemCheckEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemCheckEdit> {
        public static IModelList<IModelRepositoryItemCheckEdit> Get_ModelAdapters(IModelRepositoryItemCheckEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
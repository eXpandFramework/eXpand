using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ColorEdit")]
    public interface IModelRepositoryItemColorEdit : IModelRepositoryItem {
        IModelRepositoryItemColorEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemColorEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemColorEditModelAdapters : IModelList<IModelRepositoryItemColorEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemColorEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemColorEdit, IModelRepositoryItemColorEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemColorEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemColorEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemColorEditModelAdapter))]
    public class ModelRepositoryItemColorEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemColorEdit> {
        public static IModelList<IModelRepositoryItemColorEdit> Get_ModelAdapters(IModelRepositoryItemColorEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
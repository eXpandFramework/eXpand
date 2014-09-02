using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("FileDataEdit")]
    public interface IModelRepositoryItemFileDataEdit : IModelRepositoryItem {
        IModelRepositoryItemFileDataEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemFileDataEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemFileDataEditModelAdapters : IModelList<IModelRepositoryItemFileDataEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemFileDataEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemFileDataEdit, IModelRepositoryItemFileDataEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemFileDataEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemFileDataEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemFileDataEditModelAdapter))]
    public class ModelRepositoryItemFileDataEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemFileDataEdit> {
        public static IModelList<IModelRepositoryItemFileDataEdit> Get_ModelAdapters(IModelRepositoryItemFileDataEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("BaseSpinEdit")]
    public interface IModelRepositoryItemBaseSpinEdit : IModelRepositoryItem {
        IModelRepositoryItemBaseSpinEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemBaseSpinEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemBaseSpinEditModelAdapters : IModelList<IModelRepositoryItemBaseSpinEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemBaseSpinEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemBaseSpinEdit, IModelRepositoryItemBaseSpinEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemBaseSpinEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemBaseSpinEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemBaseSpinEditModelAdapter))]
    public class ModelRepositoryItemBaseSpinEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemBaseSpinEdit> {
        public static IModelList<IModelRepositoryItemBaseSpinEdit> Get_ModelAdapters(IModelRepositoryItemBaseSpinEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
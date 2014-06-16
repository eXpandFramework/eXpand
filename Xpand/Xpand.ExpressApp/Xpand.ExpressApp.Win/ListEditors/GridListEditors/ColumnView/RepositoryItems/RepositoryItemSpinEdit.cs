using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("SpinEdit")]
    public interface IModelRepositoryItemSpinEdit : IModelRepositoryItem {
        IModelRepositoryItemSpinEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemSpinEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemSpinEditModelAdapters : IModelList<IModelRepositoryItemSpinEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemSpinEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemSpinEdit, IModelRepositoryItemSpinEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemSpinEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemSpinEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemSpinEditModelAdapter))]
    public class ModelRepositoryItemSpinEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemSpinEdit> {
        public static IModelList<IModelRepositoryItemSpinEdit> Get_ModelAdapters(IModelRepositoryItemSpinEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("RadioGroup")]
    public interface IModelRepositoryItemRadioGroup : IModelRepositoryItem {
        IModelRepositoryItemRadioGroupModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemRadioGroupAdaptersNodeGenerator))]
    public interface IModelRepositoryItemRadioGroupModelAdapters : IModelList<IModelRepositoryItemRadioGroupModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemRadioGroupAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemRadioGroup, IModelRepositoryItemRadioGroupModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemRadioGroupModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemRadioGroup> {
    }

    [DomainLogic(typeof(IModelRepositoryItemRadioGroupModelAdapter))]
    public class ModelRepositoryItemRadioGroupModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemRadioGroup> {
        public static IModelList<IModelRepositoryItemRadioGroup> Get_ModelAdapters(IModelRepositoryItemRadioGroupModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
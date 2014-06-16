using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ProgressBar")]
    public interface IModelRepositoryItemProgressBar : IModelRepositoryItem {
        IModelRepositoryItemProgressBarModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemProgressBarAdaptersNodeGenerator))]
    public interface IModelRepositoryItemProgressBarModelAdapters : IModelList<IModelRepositoryItemProgressBarModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemProgressBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemProgressBar, IModelRepositoryItemProgressBarModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemProgressBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemProgressBar> {
    }

    [DomainLogic(typeof(IModelRepositoryItemProgressBarModelAdapter))]
    public class ModelRepositoryItemProgressBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemProgressBar> {
        public static IModelList<IModelRepositoryItemProgressBar> Get_ModelAdapters(IModelRepositoryItemProgressBarModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
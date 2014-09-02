using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("MarqueeProgressBar")]
    public interface IModelRepositoryItemMarqueeProgressBar : IModelRepositoryItem {
        IModelRepositoryItemMarqueeProgressBarModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemMarqueeProgressBarAdaptersNodeGenerator))]
    public interface IModelRepositoryItemMarqueeProgressBarModelAdapters : IModelList<IModelRepositoryItemMarqueeProgressBarModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemMarqueeProgressBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemMarqueeProgressBar, IModelRepositoryItemMarqueeProgressBarModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemMarqueeProgressBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemMarqueeProgressBar> {
    }

    [DomainLogic(typeof(IModelRepositoryItemMarqueeProgressBarModelAdapter))]
    public class ModelRepositoryItemMarqueeProgressBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemMarqueeProgressBar> {
        public static IModelList<IModelRepositoryItemMarqueeProgressBar> Get_ModelAdapters(IModelRepositoryItemMarqueeProgressBarModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
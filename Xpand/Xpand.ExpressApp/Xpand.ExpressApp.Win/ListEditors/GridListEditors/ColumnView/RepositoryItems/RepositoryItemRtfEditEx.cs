using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("RtfEditEx")]
    public interface IModelRepositoryItemRtfEditEx : IModelRepositoryItem {
        IModelRepositoryItemRtfEditExModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemRtfEditExAdaptersNodeGenerator))]
    public interface IModelRepositoryItemRtfEditExModelAdapters : IModelList<IModelRepositoryItemRtfEditExModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemRtfEditExAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemRtfEditEx, IModelRepositoryItemRtfEditExModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemRtfEditExModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemRtfEditEx> {
    }

    [DomainLogic(typeof(IModelRepositoryItemRtfEditExModelAdapter))]
    public class ModelRepositoryItemRtfEditExModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemRtfEditEx> {
        public static IModelList<IModelRepositoryItemRtfEditEx> Get_ModelAdapters(IModelRepositoryItemRtfEditExModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
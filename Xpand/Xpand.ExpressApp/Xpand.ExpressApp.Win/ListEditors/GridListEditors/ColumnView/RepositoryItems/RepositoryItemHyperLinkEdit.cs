using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("HyperLinkEdit")]
    [RepositoryItem(typeof(RepositoryItemHyperLinkEdit))]
    public interface IModelRepositoryItemHyperLinkEdit : IModelRepositoryItem {
        IModelRepositoryItemHyperLinkEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemHyperLinkEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemHyperLinkEditModelAdapters : IModelList<IModelRepositoryItemHyperLinkEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemHyperLinkEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemHyperLinkEdit, IModelRepositoryItemHyperLinkEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemHyperLinkEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemHyperLinkEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemHyperLinkEditModelAdapter))]
    public class ModelRepositoryItemHyperLinkEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemHyperLinkEdit> {
        public static IModelList<IModelRepositoryItemHyperLinkEdit> Get_ModelAdapters(IModelRepositoryItemHyperLinkEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
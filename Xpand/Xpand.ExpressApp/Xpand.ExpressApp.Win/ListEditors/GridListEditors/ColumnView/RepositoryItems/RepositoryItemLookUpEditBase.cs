using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("LookUpEditBase")]
    public interface IModelRepositoryItemLookUpEditBase : IModelRepositoryItem {
        IModelRepositoryItemLookUpEditBaseModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemLookUpEditBaseAdaptersNodeGenerator))]
    public interface IModelRepositoryItemLookUpEditBaseModelAdapters : IModelList<IModelRepositoryItemLookUpEditBaseModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemLookUpEditBaseAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemLookUpEditBase, IModelRepositoryItemLookUpEditBaseModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemLookUpEditBaseModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemLookUpEditBase> {
    }

    [DomainLogic(typeof(IModelRepositoryItemLookUpEditBaseModelAdapter))]
    public class ModelRepositoryItemLookUpEditBaseModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemLookUpEditBase> {
        public static IModelList<IModelRepositoryItemLookUpEditBase> Get_ModelAdapters(IModelRepositoryItemLookUpEditBaseModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
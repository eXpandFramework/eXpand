using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("DateEdit")]
    [RepositoryItem(typeof(RepositoryItemDateEdit))]
    public interface IModelRepositoryItemDateEdit : IModelRepositoryItem {
        IModelRepositoryItemDateEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemDateEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemDateEditModelAdapters : IModelList<IModelRepositoryItemDateEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemDateEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemDateEdit, IModelRepositoryItemDateEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemDateEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemDateEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemDateEditModelAdapter))]
    public class ModelRepositoryItemDateEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemDateEdit> {
        public static IModelList<IModelRepositoryItemDateEdit> Get_ModelAdapters(IModelRepositoryItemDateEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
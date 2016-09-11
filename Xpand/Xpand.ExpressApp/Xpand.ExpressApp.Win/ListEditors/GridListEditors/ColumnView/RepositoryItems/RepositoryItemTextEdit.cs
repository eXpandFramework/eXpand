using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("TextEdit")]
    [RepositoryItem(typeof(RepositoryItemTextEdit))]
    public interface IModelRepositoryItemTextEdit : IModelRepositoryItem {
        IModelRepositoryItemTextEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemTextEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemTextEditModelAdapters : IModelList<IModelRepositoryItemTextEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemTextEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemTextEdit, IModelRepositoryItemTextEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemTextEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemTextEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemTextEditModelAdapter))]
    public class ModelRepositoryItemTextEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemTextEdit> {
        public static IModelList<IModelRepositoryItemTextEdit> Get_ModelAdapters(IModelRepositoryItemTextEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
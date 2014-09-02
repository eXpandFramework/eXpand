using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("CheckedComboBoxEdit")]
    public interface IModelRepositoryItemCheckedComboBoxEdit : IModelRepositoryItem {
        IModelRepositoryItemCheckedComboBoxEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemCheckedComboBoxEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemCheckedComboBoxEditModelAdapters : IModelList<IModelRepositoryItemCheckedComboBoxEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemCheckedComboBoxEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemCheckedComboBoxEdit, IModelRepositoryItemCheckedComboBoxEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemCheckedComboBoxEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemCheckedComboBoxEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemCheckedComboBoxEditModelAdapter))]
    public class ModelRepositoryItemCheckedComboBoxEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemCheckedComboBoxEdit> {
        public static IModelList<IModelRepositoryItemCheckedComboBoxEdit> Get_ModelAdapters(IModelRepositoryItemCheckedComboBoxEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
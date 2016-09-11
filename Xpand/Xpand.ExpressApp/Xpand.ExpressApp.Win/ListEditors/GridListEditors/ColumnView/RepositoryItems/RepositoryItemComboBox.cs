using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ComboBox")]
    [RepositoryItem(typeof(RepositoryItemComboBox))]
    public interface IModelRepositoryItemComboBox : IModelRepositoryItem {
        IModelRepositoryItemComboBoxModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemComboBoxAdaptersNodeGenerator))]
    public interface IModelRepositoryItemComboBoxModelAdapters : IModelList<IModelRepositoryItemComboBoxModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemComboBoxAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemComboBox, IModelRepositoryItemComboBoxModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemComboBoxModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemComboBox> {
    }

    [DomainLogic(typeof(IModelRepositoryItemComboBoxModelAdapter))]
    public class ModelRepositoryItemComboBoxModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemComboBox> {
        public static IModelList<IModelRepositoryItemComboBox> Get_ModelAdapters(IModelRepositoryItemComboBoxModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
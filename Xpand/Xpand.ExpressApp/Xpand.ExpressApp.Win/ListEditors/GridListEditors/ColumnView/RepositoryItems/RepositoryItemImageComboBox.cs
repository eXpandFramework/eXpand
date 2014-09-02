using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ImageComboBox")]
    public interface IModelRepositoryItemImageComboBox : IModelRepositoryItem {
        IModelRepositoryItemImageComboBoxModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemImageComboBoxAdaptersNodeGenerator))]
    public interface IModelRepositoryItemImageComboBoxModelAdapters : IModelList<IModelRepositoryItemImageComboBoxModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemImageComboBoxAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemImageComboBox, IModelRepositoryItemImageComboBoxModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemImageComboBoxModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemImageComboBox> {
    }

    [DomainLogic(typeof(IModelRepositoryItemImageComboBoxModelAdapter))]
    public class ModelRepositoryItemImageComboBoxModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemImageComboBox> {
        public static IModelList<IModelRepositoryItemImageComboBox> Get_ModelAdapters(IModelRepositoryItemImageComboBoxModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
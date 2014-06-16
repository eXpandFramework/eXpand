using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("IModelRepositoryFieldPicker")]
    public interface IModelRepositoryFieldPicker : IModelRepositoryItem {
        IModelRepositoryFieldPickerModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryFieldPickerAdaptersNodeGenerator))]
    public interface IModelRepositoryFieldPickerModelAdapters : IModelList<IModelRepositoryFieldPickerModelAdapter>, IModelNode {

    }

    public class ModelRepositoryFieldPickerAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryFieldPicker, IModelRepositoryFieldPickerModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryFieldPickerModelAdapter : IModelCommonModelAdapter<IModelRepositoryFieldPicker> {
    }

    [DomainLogic(typeof(IModelRepositoryFieldPickerModelAdapter))]
    public class ModelRepositoryFieldPickerModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryFieldPicker> {
        public static IModelList<IModelRepositoryFieldPicker> Get_ModelAdapters(IModelRepositoryFieldPickerModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
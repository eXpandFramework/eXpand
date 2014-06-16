using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("ColorPickEdit")]
    public interface IModelRepositoryItemColorPickEdit : IModelRepositoryItem {
        IModelRepositoryItemColorPickEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemColorPickEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemColorPickEditModelAdapters : IModelList<IModelRepositoryItemColorPickEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemColorPickEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemColorPickEdit, IModelRepositoryItemColorPickEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemColorPickEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemColorPickEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemColorPickEditModelAdapter))]
    public class ModelRepositoryItemColorPickEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemColorPickEdit> {
        public static IModelList<IModelRepositoryItemColorPickEdit> Get_ModelAdapters(IModelRepositoryItemColorPickEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
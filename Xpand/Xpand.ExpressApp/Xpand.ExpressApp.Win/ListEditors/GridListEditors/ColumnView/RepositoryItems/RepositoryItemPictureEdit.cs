using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("PictureEdit")]
    public interface IModelRepositoryItemPictureEdit : IModelRepositoryItem {
        IModelRepositoryItemPictureEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemPictureEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemPictureEditModelAdapters : IModelList<IModelRepositoryItemPictureEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemPictureEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemPictureEdit, IModelRepositoryItemPictureEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemPictureEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemPictureEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemPictureEditModelAdapter))]
    public class ModelRepositoryItemPictureEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemPictureEdit> {
        public static IModelList<IModelRepositoryItemPictureEdit> Get_ModelAdapters(IModelRepositoryItemPictureEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("FontEdit")]
    public interface IModelRepositoryItemFontEdit : IModelRepositoryItem {
        IModelRepositoryItemFontEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemFontEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemFontEditModelAdapters : IModelList<IModelRepositoryItemFontEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemFontEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemFontEdit, IModelRepositoryItemFontEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemFontEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemFontEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemFontEditModelAdapter))]
    public class ModelRepositoryItemFontEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemFontEdit> {
        public static IModelList<IModelRepositoryItemFontEdit> Get_ModelAdapters(IModelRepositoryItemFontEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
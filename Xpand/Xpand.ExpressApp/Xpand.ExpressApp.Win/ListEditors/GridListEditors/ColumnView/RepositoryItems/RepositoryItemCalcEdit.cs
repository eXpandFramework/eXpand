using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
    [ModelDisplayName("CalcEdit")]
    public interface IModelRepositoryItemCalcEdit : IModelRepositoryItem {
        IModelRepositoryItemCalcEditModelAdapters ModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelRepositoryItemCalcEditAdaptersNodeGenerator))]
    public interface IModelRepositoryItemCalcEditModelAdapters : IModelList<IModelRepositoryItemCalcEditModelAdapter>, IModelNode {

    }

    public class ModelRepositoryItemCalcEditAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemCalcEdit, IModelRepositoryItemCalcEditModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelRepositoryItemCalcEditModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemCalcEdit> {
    }

    [DomainLogic(typeof(IModelRepositoryItemCalcEditModelAdapter))]
    public class ModelRepositoryItemCalcEditModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemCalcEdit> {
        public static IModelList<IModelRepositoryItemCalcEdit> Get_ModelAdapters(IModelRepositoryItemCalcEditModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }
}
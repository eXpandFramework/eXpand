//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.XtraEditors.Repository;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems{
//    [ModelDisplayName("BaseProgressBar")]
//    [RepositoryItem(typeof(RepositoryItemBaseProgressBar))]
//    public interface IModelRepositoryItemBaseProgressBar : IModelRepositoryItem {
//        IModelRepositoryItemBaseProgressBarModelAdapters ModelAdapters { get; }
//    }
//
//    [ModelNodesGenerator(typeof(ModelRepositoryItemBaseProgressBarAdaptersNodeGenerator))]
//    public interface IModelRepositoryItemBaseProgressBarModelAdapters : IModelList<IModelRepositoryItemBaseProgressBarModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelRepositoryItemBaseProgressBarAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelRepositoryItemBaseProgressBar, IModelRepositoryItemBaseProgressBarModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelRepositoryItemBaseProgressBarModelAdapter : IModelCommonModelAdapter<IModelRepositoryItemBaseProgressBar> {
//    }
//
//    [DomainLogic(typeof(IModelRepositoryItemBaseProgressBarModelAdapter))]
//    public class ModelRepositoryItemBaseProgressBarModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelRepositoryItemBaseProgressBar> {
//        public static IModelList<IModelRepositoryItemBaseProgressBar> Get_ModelAdapters(IModelRepositoryItemBaseProgressBarModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }
//}
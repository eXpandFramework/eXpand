using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.PropertyEditors{

//    [ModelAbstractClass]
//    public interface IModelMemberViewItemDashboardViewer : IModelMemberViewItem{
//        [ModelBrowsable(typeof(DashboardViewerVisibilityCalculator))]
//        IModelDashboardViewer DashboardViewr { get; }
//    }

//    public class DashboardViewerVisibilityCalculator : EditorTypeVisibilityCalculator<IDashboardViewEditor,IModelMemberViewItem> {
//    }
//
    public interface IDashboardViewEditor  {
    }
//
//    public interface IModelDashboardViewEditor : IModelModelAdapter {
//        IModelDashboardViewer DashboardViewer { get; }
//        
//    }
//
//    [ModelNodesGenerator(typeof(ModelDashboardViewerAdaptersNodeGenerator))]
//    public interface IModelDashboardViewerModelAdapters : IModelList<IModelDashboardViewerModelAdapter>, IModelNode {
//
//    }
//
//    public class ModelDashboardViewerAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelDashboardViewer, IModelDashboardViewerModelAdapter> {
//    }
//
//    [ModelDisplayName("Adapter")]
//    public interface IModelDashboardViewerModelAdapter : IModelCommonModelAdapter<IModelDashboardViewer> {
//    }
//
//    [DomainLogic(typeof(IModelDashboardViewerModelAdapter))]
//    public class ModelDashboardViewerModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelDashboardViewer> {
//        public static IModelList<IModelDashboardViewer> Get_ModelAdapters(IModelDashboardViewerModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }

//    public interface IModelDashboardViewer : IModelModelAdapter {
//        IModelDashboardViewerModelAdapters ModelAdapters { get; }
//    }

//    public abstract class DashboardViewerModelAdapter : PropertyEditorControlAdapterController<IModelMemberViewItemDashboardViewer, IModelDashboardViewer, IDashboardViewEditor> {
//        protected override Expression<Func<IModelMemberViewItemDashboardViewer, IModelModelAdapter>> GetControlModel(IModelMemberViewItemDashboardViewer modelMemberViewItemFilterControl){
//            return viewer => viewer.DashboardViewr;
//        }
//    }
}
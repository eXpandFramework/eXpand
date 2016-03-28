using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.PropertyEditors{

    [ModelAbstractClass]
    public interface IModelPropertyEditorDashboardViewer : IModelPropertyEditor{
        [ModelBrowsable(typeof(DashboardViewerVisibilityCalculator))]
        IModelDashboardViewer DashboardViewr { get; }
    }

    public class DashboardViewerVisibilityCalculator : EditorTypeVisibilityCalculator<IDashboardViewEditor,IModelPropertyEditor> {
    }

    public interface IDashboardViewEditor  {
    }

    public interface IModelDashboardViewEditor : IModelModelAdapter {
        IModelDashboardViewer DashboardViewer { get; }
        
    }

    [ModelNodesGenerator(typeof(ModelDashboardViewerAdaptersNodeGenerator))]
    public interface IModelDashboardViewerModelAdapters : IModelList<IModelDashboardViewerModelAdapter>, IModelNode {

    }

    public class ModelDashboardViewerAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelDashboardViewer, IModelDashboardViewerModelAdapter> {
    }

    [ModelDisplayName("Adapter")]
    public interface IModelDashboardViewerModelAdapter : IModelCommonModelAdapter<IModelDashboardViewer> {
    }

    [DomainLogic(typeof(IModelDashboardViewerModelAdapter))]
    public class ModelDashboardViewerModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelDashboardViewer> {
        public static IModelList<IModelDashboardViewer> Get_ModelAdapters(IModelDashboardViewerModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public interface IModelDashboardViewer : IModelModelAdapter {
        IModelDashboardViewerModelAdapters ModelAdapters { get; }
    }

    public abstract class DashboardViewerModelAdapter : PropertyEditorControlAdapterController<IModelPropertyEditorDashboardViewer, IModelDashboardViewer, IDashboardViewEditor> {
        protected override Expression<Func<IModelPropertyEditorDashboardViewer, IModelModelAdapter>> GetControlModel(IModelPropertyEditorDashboardViewer modelPropertyEditorFilterControl){
            return viewer => viewer.DashboardViewr;
        }
    }
}
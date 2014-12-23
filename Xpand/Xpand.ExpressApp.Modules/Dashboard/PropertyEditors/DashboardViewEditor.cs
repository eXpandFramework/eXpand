using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.PropertyEditors{

    [ModelAbstractClass]
    public interface IModelPropertyEditorDashboardViewer : IModelPropertyEditor{
        [ModelBrowsable(typeof(DashboardViewerEditorVisibilityCalculator))]
        IModelDashboardViewEditor DashboardViewEditor { get; }
    }

    public class DashboardViewerEditorVisibilityCalculator : EditorTypeVisibilityCalculator<IDashboardViewEditor,IModelPropertyEditor> {
    }

    public interface IDashboardViewEditor : IModelPropertyEditorControlAdapter {
    }

    public interface IModelDashboardViewEditor : IModelNode {
        IModelDashboardViewer DashboardViewer { get; }
        IModelDashboardViewerModelAdapters ModelAdapters { get; }
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

    }

    public abstract class DashboardViewerModelAdapter : PropertyEditorControlAdapterController<IModelPropertyEditorDashboardViewer, IModelDashboardViewer> {
        protected override IModelDashboardViewer[] GetControlModelNodes(IModelPropertyEditorDashboardViewer modelPropertyEditorLabelControl){
            var modelDashboardViewers = modelPropertyEditorLabelControl.DashboardViewEditor.ModelAdapters.Select(adapter => adapter.ModelAdapter);
            return modelDashboardViewers.Concat(new[] { modelPropertyEditorLabelControl.DashboardViewEditor.DashboardViewer }).ToArray();
        }
    }
}
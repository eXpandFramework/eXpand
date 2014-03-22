using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.PropertyEditors{
    [ModelAbstractClass]
    public interface IModelPropertyEditorDashboardViewer : IModelPropertyEditor {
        [ModelBrowsable(typeof(DashboardViewerEditorVisibilityCalculator))]
        IModelDashboardViewEditor DashboardViewEditor { get; }
    }

    public class DashboardViewerEditorVisibilityCalculator : EditorTypeVisibilityCalculator<IDashboardViewEditor> {
    }

    public interface IDashboardViewEditor {
    }

    public interface IModelDashboardViewEditor : IModelNode {
        IModelDashboardViewer DashboardViewer { get; }
    }

    

    public interface IModelDashboardViewer : IModelNodeEnabled {

    }

    public abstract class DashboardViewerModelAdapter : PropertyEditorControlAdapterController<IModelPropertyEditorDashboardViewer, IModelDashboardViewer> {
        protected override IModelDashboardViewer GetControlModelNode(IModelPropertyEditorDashboardViewer modelPropertyEditorLabelControl) {
            return modelPropertyEditorLabelControl.DashboardViewEditor.DashboardViewer;
        }
    }
}
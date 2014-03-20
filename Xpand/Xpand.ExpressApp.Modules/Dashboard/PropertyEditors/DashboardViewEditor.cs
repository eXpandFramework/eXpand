using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
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
        [DefaultValue(true)]
        [Category("eXpand.XtraDashoard.Win")]
        bool AllowPrintDashboard { get; set; }
        [Category("eXpand.XtraDashoard.Win")]
        [DefaultValue(true)]
        bool AllowPrintDashboardItems { get; set; }

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
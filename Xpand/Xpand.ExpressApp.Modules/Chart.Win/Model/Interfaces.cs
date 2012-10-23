using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraCharts;
using System.Linq;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Chart.Win.Model {

    public interface IModelOptionsChart : IModelOptionsColumnView {
        IModelSeriesCollection Series { get; }
        IModelChartHitInfo HotTrackHitInfo { get; }
        IModelChartHitInfo SelectionHitInfo { get; }
        IModelChartDiagrams Diagrams { get; }
    }

    public interface IModelChartHitInfo : IModelNodeEnabled {
        bool? InAnnotation { get; set; }
        bool? InAxis { get; set; }
        bool? InChart { get; set; }
        bool? InChartTitle { get; set; }
        bool? InConstantLine { get; set; }
        bool? InDiagram { get; set; }
        bool? InIndicator { get; set; }
        bool? InLegend { get; set; }
        bool? InNonDefaultPane { get; set; }
        bool? InSeries { get; set; }
        bool? InSeriesLabel { get; set; }
        bool? InSeriesTitle { get; set; }
    }
    public interface IModelSeriesCollection : IModelNode, IModelList<IModelSeries> {
    }

    public interface IModelSeries : IModelNode {
        IModelDataFilters DataFilters { get; }
        [DataSourceProperty("DataSourceListViews")]
        IModelListView DataSourceListView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> DataSourceListViews { get; }
    }

    [DomainLogic(typeof(IModelSeries))]
    public class IModelSeriesDomainLogic {
        public static IModelList<IModelListView> Get_DataSourceListViews(IModelSeries modelSeries) {
            return new CalculatedModelNodeList<IModelListView>(modelSeries.Application.Views.OfType<IModelListView>());
        }
    }

    public interface IModelDataFilters : IModelNode, IModelList<IModelDataFilter> {
    }

    public interface IModelChartDiagrams : IModelNode, IModelList<IModelChartDiargam> {
    }

    [ModelAbstractClass]
    public interface IModelChartDiargam : IModelNodeEnabled {
    }

    [ModelDisplayName("Diagram3D")]
    public interface IModelChartDiagram3D : IModelChartDiargam {
    }

    [ModelDisplayName("SimpleDiagram3D")]
    public interface IModelChartSimpleDiagram3D : IModelChartDiargam {
    }

    [ModelDisplayName("FunnelDiagram3D")]
    public interface IModelChartFunnelDiagram3D : IModelChartDiargam {
    }

    [ModelDisplayName("XYDiagram3D")]
    public interface IModelChartXYDiagram3D : IModelChartDiargam {
    }

    [ModelDisplayName("RadarDiagram3D")]
    public interface IModelChartRadarDiagram : IModelChartDiargam {
    }

    [ModelDisplayName("PollarDiagram")]
    public interface IModelChartPolarDiagram : IModelChartDiargam {
    }

    [ModelDisplayName("SimpleDiagram")]
    public interface IModelChartSimpleDiagram : IModelChartDiargam {
    }

    [ModelDisplayName("XYDiagram2D")]
    public interface IModelChartXYDiagram2D : IModelChartDiargam {
    }

    [ModelDisplayName("SwiftPlotDiagram")]
    public interface IModelChartSwiftPlotDiagram : IModelChartDiargam {
    }

    [ModelDisplayName("XYDiagram")]
    public interface IModelChartXYDiagram : IModelChartDiargam {
    }

    [ModelDisplayName("GanttDiagram")]
    public interface IModelChartGanttDiagram : IModelChartDiargam {
    }

    public interface IModelDataFilter : IModelNode {
        string ColumnName { get; set; }
        [TypeConverter(typeof(StringToTypeConverterExtended))]
        Type DataType { get; set; }
        DataFilterCondition? FilterCondition { get; set; }
        decimal Value { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsChart : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(ChartEditorVisibilityCalculator))]
        IModelOptionsChart OptionsChart { get; }
    }

    public class ChartEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            Type editorType = EditorType(node);
            return new[] { typeof(ChartListEditor), typeof(PivotGridListEditor) }.Any(type => type.IsAssignableFrom(editorType));
        }
        #endregion
    }

}

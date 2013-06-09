using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotGrid.Win;
using System.Linq;
using DevExpress.XtraPivotGrid;
using Xpand.ExpressApp.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Chart.Win.Model {
    public class PivotGridListEditorChartModelSynchronizer : ModelListSynchronizer {
        public PivotGridListEditorChartModelSynchronizer(ListEditor columnViewEditor, XafApplication application)
            : base(columnViewEditor, columnViewEditor.Model) {
            ModelSynchronizerList.Add(new ChartControlSynchronizer(ChartControl(columnViewEditor), ((IModelListViewOptionsChart)columnViewEditor.Model).OptionsChart, application));
        }

        static DevExpress.XtraCharts.ChartControl ChartControl(ListEditor columnViewEditor) {
            var pivotGridListEditor = columnViewEditor as PivotGridListEditor;
            return pivotGridListEditor != null ? pivotGridListEditor.ChartControl : ((ChartListEditor)columnViewEditor).ChartControl;
        }
    }

    public class ChartControlSynchronizer : ComponentSynchronizer<DevExpress.XtraCharts.ChartControl, IModelOptionsChart> {
        readonly XafApplication _application;

        public ChartControlSynchronizer(DevExpress.XtraCharts.ChartControl chartControl, IModelOptionsChart modelOptionsChart, XafApplication application)
            : base(chartControl, modelOptionsChart, false) {
            _application = application;
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            ApplySeriesModel();
            foreach (var modelChartDiagram in GetModelDiagrams()) {
                ApplyModel(modelChartDiagram, Control.Diagram, ApplyValues);
            }
        }

        IEnumerable<IModelChartDiargam> GetModelDiagrams() {
            if (Control.Diagram != null) {
                var name = Control.Diagram.GetType().Name;
                return Model.Diagrams.Where(diargam => diargam.NodeEnabled && diargam.GetType().Name.EndsWith(name));
            }
            return Enumerable.Empty<IModelChartDiargam>();
        }

        void ApplySeriesModel() {
            var modelSeries = Model.Series.Where(series => series.DataSourceListView != Model.Parent);
            foreach (var grouping in modelSeries.GroupBy(series => series.DataSourceListView)) {
                var pivotGridControl = GetPivotGridControl(grouping);
                foreach (IModelSeries serie in grouping) {
                    var chartSeries = Control.Series[serie.GetValue<string>("Id")];
                    if (chartSeries != null) {
                        chartSeries.DataSource = pivotGridControl;
                        ApplyModel(serie, chartSeries, ApplyValues);
                    }
                }
            }
        }

        PivotGridControl GetPivotGridControl(IGrouping<IModelListView, IModelSeries> grouping) {
            var modelListView = grouping.Key;
            if (modelListView != null) {
                var objectSpace = _application.CreateObjectSpace(modelListView.ModelClass.TypeInfo.Type);
                var collectionSource = _application.CreateCollectionSource(objectSpace, modelListView.ModelClass.TypeInfo.Type, modelListView.Id);
                var listView = _application.CreateListView(modelListView, collectionSource, true);
                var window = _application.CreateWindow(TemplateContext.View, null, true, false);
                window.SetView(listView);
                return ((PivotGridListEditor)listView.Editor).PivotGridControl;
            }
            return null;
        }

        public override void SynchronizeModel() {

        }
    }

}
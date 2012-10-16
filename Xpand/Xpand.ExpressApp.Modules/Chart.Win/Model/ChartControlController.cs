using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraCharts;
using System.Linq;

namespace Xpand.ExpressApp.Chart.Win.Model {
    public class ChartControlController : ViewController<ListView> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (ChartControl != null) {
                ChartControl.ObjectHotTracked += ChartControlOnObjectHotTracked;
                ChartControl.ObjectSelected += ChartControlOnObjectSelected;
            }
        }

        void ChartControlOnObjectSelected(object sender, HotTrackEventArgs hotTrackEventArgs) {
            var modelChartHitInfo = ((IModelListViewOptionsChart)View.Model).OptionsChart.SelectionHitInfo;
            ApplyHitInfoRules(hotTrackEventArgs, modelChartHitInfo);
            if (!hotTrackEventArgs.Cancel && hotTrackEventArgs.HitInfo.InSeries) {
                var argumentDataMember = View.ObjectTypeInfo.FindMember(((SeriesBase)hotTrackEventArgs.Object).ArgumentDataMember);
                if (argumentDataMember != null) {
                    var collectionSource = Application.CreateCollectionSource(ObjectSpace, View.ObjectTypeInfo.Type, View.Model.Id, CollectionSourceMode.Normal);
                    var argumentValue = ReflectionHelper.Convert(hotTrackEventArgs.HitInfo.SeriesPoint.Argument, argumentDataMember.MemberType);
                    var criteriaOperator = CriteriaOperator.Parse(argumentDataMember.Name + "=?", argumentValue);

                    collectionSource.Criteria["arg"] = criteriaOperator;
                    var customSelectedObjects = ((ISelectionCriteria)View.Editor);
                    customSelectedObjects.SelectionCriteria = criteriaOperator;
                    customSelectedObjects.AddSelectedObjects(((IEnumerable)collectionSource.Collection).Cast<object>());
                }
            }
        }

        void ChartControlOnObjectHotTracked(object sender, HotTrackEventArgs hotTrackEventArgs) {
            var modelChartHitInfo = ((IModelListViewOptionsChart)View.Model).OptionsChart.HotTrackHitInfo;
            ApplyHitInfoRules(hotTrackEventArgs, modelChartHitInfo);
        }

        void ApplyHitInfoRules(HotTrackEventArgs hotTrackEventArgs, IModelChartHitInfo modelChartHitInfo) {
            if (modelChartHitInfo.NodeEnabled) {
                var chartHitInfo = hotTrackEventArgs.HitInfo;
                foreach (var propertyInfo in typeof(IModelChartHitInfo).GetProperties()) {
                    var value = propertyInfo.GetValue(modelChartHitInfo, null);
                    if (value != null) {
                        var property = chartHitInfo.GetType().GetProperty(propertyInfo.Name);
                        if (property != null) {
                            var hitInfoValue = (bool)property.GetValue(chartHitInfo, null);
                            if (hitInfoValue) {
                                hotTrackEventArgs.Cancel = !(bool)value;
                            }
                        }
                    }
                }
            }
        }

        public ChartControl ChartControl {
            get {
                return PivotGridListEditor != null ? PivotGridListEditor.ChartControl
                           : (ChartListEditor != null ? ChartListEditor.ChartControl : null);
            }
        }

        PivotGridListEditor PivotGridListEditor {
            get { return View != null ? View.Editor as PivotGridListEditor : null; }
        }
        ChartListEditor ChartListEditor {
            get { return View != null ? View.Editor as ChartListEditor : null; }
        }


    }
}
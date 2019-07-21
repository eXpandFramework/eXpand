using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraCharts;
using System.Linq;
using Xpand.Persistent.Base.General;
using Fasterflect;

namespace Xpand.ExpressApp.Chart.Win.Model {
    public class ChartControlController : ViewController<ListView> {
        private IModelOptionsChartEx _modelOptionsChartEx;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (ChartControl != null) {
                _modelOptionsChartEx = ((IModelOptionsChartEx) View.Model.GetNode(XpandChartWinModule.ChartControlMapName));
                ChartControl.ObjectHotTracked += ChartControlOnObjectHotTracked;
                ChartControl.ObjectSelected += ChartControlOnObjectSelected;
            }
        }
        
        void ChartControlOnObjectSelected(object sender, HotTrackEventArgs hotTrackEventArgs) {
            ApplyHitInfoRules(hotTrackEventArgs, _modelOptionsChartEx.HotTrackHitInfo);
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
            var modelChartHitInfo = _modelOptionsChartEx.HotTrackHitInfo;
            ApplyHitInfoRules(hotTrackEventArgs, modelChartHitInfo);
        }

        void ApplyHitInfoRules(HotTrackEventArgs hotTrackEventArgs, IModelChartHitInfo modelChartHitInfo) {
            if (modelChartHitInfo.NodeEnabled) {
                var chartHitInfo = hotTrackEventArgs.HitInfo;
                foreach (var propertyInfo in typeof(IModelChartHitInfo).GetProperties()) {
                    var value = propertyInfo.GetValue(modelChartHitInfo, null);
                    if (value != null) {
                        var hitInfoValue = (bool)chartHitInfo.GetPropertyValue(propertyInfo.Name);
                        if (hitInfoValue) {
                            hotTrackEventArgs.Cancel = !(bool) value;
                        }
                    }
                }
            }
        }

        public ChartControl ChartControl =>PivotGridListEditor != null ? PivotGridListEditor.ChartControl: ChartListEditor?.ChartControl;

        PivotGridListEditor PivotGridListEditor => View?.Editor as PivotGridListEditor;

        ChartListEditor ChartListEditor => View?.Editor as ChartListEditor;
    }
}
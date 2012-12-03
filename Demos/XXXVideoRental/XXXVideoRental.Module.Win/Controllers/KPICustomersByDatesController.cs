using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraCharts;
using XXXVideoRental.Module.Win.BusinessObjects;

namespace XXXVideoRental.Module.Win.Controllers {
    public class KPICustomersByDatesController : ViewController<ListView> {
        private const string Payment = "Payment";
        private const string Customer = "Customer";
        public KPICustomersByDatesController() {
            TargetViewId = ViewIdProvider.CustomersKPICustomersByDates;
            var sortCustomersByDatesChart = new SingleChoiceAction(this, "SortCustomersByDatesChart", PredefinedCategory.Filters);
            sortCustomersByDatesChart.Items.Add(new ChoiceActionItem(Payment, null));
            sortCustomersByDatesChart.Items.Add(new ChoiceActionItem(Customer, null));
            sortCustomersByDatesChart.Execute += SortCustomersByDatesChartOnExecute;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            PivotGridListEditor.ChartControl.CustomDrawSeriesPoint += CustomDrawSeriesPoint;
            PivotGridListEditor.ChartControl.BoundDataChanged += ChartControlOnBoundDataChanged;
        }

        void ChartControlOnBoundDataChanged(object sender, EventArgs eventArgs) {
            var chartControl = PivotGridListEditor.ChartControl;
            var diagram = chartControl.Diagram as XYDiagram;
            if (diagram != null && chartControl.Series.Count > 0) {
                chartControl.Titles[0].Text = View.Model.Caption + " (" + chartControl.Series[0].Name + ")";
            }
        }

        void CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e) {
            var d = e.SeriesPoint.Values[0];
            if (d >= (int)DiscountLevelCalculator.CustomerDiscountLevel[(int)DiscountLevel.Prodigious]) {
                SetColor(e, DiscountLevel.Prodigious);
            } else if (d >= (int)DiscountLevelCalculator.CustomerDiscountLevel[(int)DiscountLevel.Active])
                SetColor(e, DiscountLevel.Active);
            else if (d >= (int)DiscountLevelCalculator.CustomerDiscountLevel[(int)DiscountLevel.Occasional])
                SetColor(e, DiscountLevel.Occasional);
            else if (d >= (int)DiscountLevelCalculator.CustomerDiscountLevel[(int)DiscountLevel.Basic]) {
                SetColor(e, DiscountLevel.Basic);
            } else
                SetColor(e, DiscountLevel.FirstTime);
            ((BarDrawOptions)e.SeriesDrawOptions).FillStyle.FillMode = FillMode.Solid;
        }

        void SetColor(CustomDrawSeriesPointEventArgs e, DiscountLevel discountLevel) {
            e.SeriesDrawOptions.Color = PivotGridListEditor.ChartControl.Series[discountLevel.ToString()].View.Color;
        }

        void SortCustomersByDatesChartOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var seriesTemplate = PivotGridListEditor.ChartControl.SeriesTemplate;
            if (e.SelectedChoiceActionItem.Id == Payment) {
                seriesTemplate.SeriesPointsSortingKey = SeriesPointKey.Value_1;
                seriesTemplate.SeriesPointsSorting = SortingMode.Descending;
            } else {
                seriesTemplate.SeriesPointsSortingKey = SeriesPointKey.Argument;
                seriesTemplate.SeriesPointsSorting = SortingMode.Ascending;
            }
        }

        PivotGridListEditor PivotGridListEditor {
            get { return View != null ? View.Editor as PivotGridListEditor : null; }
        }

    }
}

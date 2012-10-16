using System;
using System.Windows.Forms;
using DevExpress.XtraGauges.Win;

namespace Xpand.ExpressApp.PivotGrid.Win.NetIncome {
    public partial class NetIncomeGaugeControl : UserControl, IPivotGaugeTemplate {
        public NetIncomeGaugeControl() {
            InitializeComponent();
        }

        public NetIncomeGaugeControl(IModelNetIncome modelNetIncome)
            : this() {
            labelComponent1.Text = modelNetIncome.RowFieldName;
        }
        #region Implementation of IPivotGaugeTemplate
        public GaugeControl GaugeControl {
            get { return gaugeControl2; }

        }
        #endregion
        public void UpdateGauge(double value, string text, double[] values, float overhead, string gaugeTextFormat) {
            double min = 2500;
            double max = 4500;
            double avg = 3000;
            if (values.Length > 0) {
                double sum = 0;
                min = values[0]; max = values[0];
                foreach (double t in values) {
                    min = Math.Min(min, t);
                    max = Math.Max(max, t);
                    sum += t;
                }
                avg = sum / values.Length;
            }
            //            float overhead = radioGroup1.SelectedIndex == 0 ? 100f : 10f;

            arcScaleComponent1.Labels[0].FormatString = (Math.Abs(value - 0d) < double.Epsilon || value > max || value < min) ? string.Empty : gaugeTextFormat;

            arcScaleComponent1.MinValue = (float)min - overhead;
            arcScaleComponent1.Value = (float)value;
            arcScaleComponent1.MaxValue = (float)max + overhead;

            arcScaleComponent1.Ranges[0].StartValue = (float)min;
            arcScaleComponent1.Ranges[0].EndValue = (float)avg;
            arcScaleComponent1.Ranges[1].StartValue = (float)avg;
            arcScaleComponent1.Ranges[1].EndValue = (float)max;

            labelComponent1.Text = text;
        }
    }
}

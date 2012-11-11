using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.Data.PivotGrid;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.LookAndFeel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Skins;
using DevExpress.XtraCharts;
using DevExpress.XtraGauges.Win;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPivotGrid;
using System.Linq;
using Xpand.ExpressApp.PivotGrid.Win.Model;

namespace Xpand.ExpressApp.PivotGrid.Win.NetIncome {
    public class CurrentMonthFieldSort : IPivotFieldSortEvent {
        #region Implementation of IPivotFieldSortEvent
        public void Calculate(PivotGridCustomFieldSortEventArgs e) {
            e.Result = Comparer<int>.Default.Compare(CorrectMonth((int)e.Value1), CorrectMonth((int)e.Value2));
            e.Handled = true;
        }
        int CorrectMonth(int month) {
            return month > DateTime.Now.Month ? month - DateTime.Now.Month : month + 12;
        }
        #endregion
    }

    [ModelAbstractClass]
    public interface IModelPivotNetIncome : IModelOptionsPivotGrid {
        IModelNetIncome NetIncome { get; }
    }

    public enum PivotListEditorControlType {
        Pivot,
        Chart
    }

    public interface IModelPivotGauge : IModelNode {
        bool Enabled { get; set; }
        [RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Enabled=true")]
        PivotListEditorControlType ControlType { get; set; }
        InsertType InsertType { get; set; }
        [DataSourceProperty("GaugeProviderTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Enabled=true")]
        Type GaugeProviderType { get; set; }
        [Browsable(false)]
        IEnumerable<Type> GaugeProviderTypes { get; }
    }

    [DomainLogic(typeof(IModelPivotGauge))]
    public class PivotFieldSortRuleDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_GaugeProviderTypes(IModelPivotGauge modelPivotGauge) {
            return FindTypeDescenants(typeof(IPivotGaugeTemplate));
        }
    }

    public interface IPivotGaugeTemplate {
        GaugeControl GaugeControl { get; }
        void UpdateGauge(double value, string text, double[] values, float overhead, string gaugeTextFormat);
    }

    public interface IModelNetIncome : IModelNode {
        IModelPivotGauge Gauge { get; }
        bool Enabled { get; set; }
        [DefaultValue("Period")]
        string ColumnFieldName { get; set; }
        [DefaultValue("Month")]
        string RowFieldName { get; set; }
        [DefaultValue("Revenue")]
        string DataFieldName { get; set; }
        [DefaultValue("Time Period ({0:d} - {1:d})")]
        string TimePeriod { get; set; }
        [DefaultValue("Period Comparison ({0:d} - {1:d})")]
        string ComparePeriod { get; set; }
        [DefaultValue(-1)]
        int CurrentPeriodDateInYears { get; set; }
        [DefaultValue(-2)]
        int StartDateInYears { get; set; }
        IModelPivotDataFieldRules DataFieldRules { get; }
    }

    public interface IModelPivotDataFieldRules : IModelList<IModelPivotDataFieldRule>, IModelNode {

    }
    public interface IModelPivotDataFieldRule : IModelNode {
        bool Enabled { get; set; }
        [DefaultValue(PivotSummaryType.Sum)]
        PivotSummaryType SummaryType { get; set; }
        [DataSourceProperty("ViewVariants")]
        IModelVariant ViewVariant { get; set; }
        [DefaultValue("${1:F2}")]
        string GaugeTextFormat { get; set; }
        [DefaultValue(100f)]
        float OverHead { get; set; }
        [Browsable(false)]
        IModelList<IModelVariant> ViewVariants { get; }
    }
    [DomainLogic(typeof(IModelPivotDataFieldRule))]
    public class PivotDataFieldRuleDomainLogic {
        public static IModelList<IModelVariant> Get_ViewVariants(IModelPivotDataFieldRule modelPivotDataFieldRule) {
            return ((IModelViewVariants)modelPivotDataFieldRule.Parent.Parent.Parent.Parent).Variants;
        }
    }

    public class NetIncomeController : PivotGridControllerBase, IModelExtender, IPivotGroupIntervalEvent {
        const int CurrentPeriod = 1, ComparePeriod = 2;
        DateTime _currentPeriodDate;
        string _comparePeriodText;
        string _currentPeriodText;
        IPivotGaugeTemplate _pivotGaugeTemplate;

        protected override void OnActivated() {
            base.OnActivated();
            if (IsActive()) {
                var modelNetIncome = Model.NetIncome;
                _currentPeriodDate = DateTime.Now.AddYears(modelNetIncome.CurrentPeriodDateInYears);
                var startDate = DateTime.Now.AddYears(modelNetIncome.StartDateInYears);
                _currentPeriodText = string.Format(modelNetIncome.TimePeriod, _currentPeriodDate, DateTime.Now.AddDays(-1));
                _comparePeriodText = string.Format(modelNetIncome.ComparePeriod, startDate, _currentPeriodDate.AddDays(-1));
            }
        }
        protected override bool IsActive() {
            return base.IsActive() && Model.NetIncome.Enabled;
        }

        protected override void AttachToControlEvents() {
            var pivotGridControl = PivotGridListEditor.PivotGridControl;
            pivotGridControl.FieldValueDisplayText += PivotGridControlOnFieldValueDisplayText;
            pivotGridControl.CustomDrawCell += PivotGridControlOnCustomDrawCell;
            pivotGridControl.FocusedCellChanged += PivotGridControlOnFocusedCellChanged;
        }

        void PivotGridControlOnFocusedCellChanged(object sender, EventArgs eventArgs) {
            var doubles = GetValues(PivotGridListEditor.ChartControl.Series[0]);
            var pivotGrid = PivotGridListEditor.PivotGridControl;
            var value = Convert.ToDouble(pivotGrid.GetCellValue(pivotGrid.Cells.FocusedCell.X, pivotGrid.Cells.FocusedCell.Y));
            if (_pivotGaugeTemplate != null)
                _pivotGaugeTemplate.UpdateGauge(value, GetGaugeTextFromPivot(), doubles, PivotDataFieldRule.OverHead, PivotDataFieldRule.GaugeTextFormat);
        }

        IModelPivotDataFieldRule PivotDataFieldRule {
            get {
                var modelPivotDataFieldRule = Model.NetIncome.DataFieldRules.FirstOrDefault(rule => rule.Enabled && rule.ViewVariant.Id == GetCurrentVariantId());
                if (modelPivotDataFieldRule == null)
                    throw new NotImplementedException("Add at least one " + typeof(IModelPivotDataFieldRule).Name);
                return modelPivotDataFieldRule;
            }
        }

        string GetCurrentVariantId() {
            return ((IModelViewVariants)View.Model).Variants.Current != null ? ((IModelViewVariants)View.Model).Variants.Current.Id : null;
        }

        string GetGaugeTextFromPivot() {
            var pivotGridControl = PivotGridListEditor.PivotGridControl;
            var pivotGridField = pivotGridControl.Fields[Model.NetIncome.RowFieldName];
            object fieldValue = pivotGridControl.GetFieldValue(pivotGridField, pivotGridControl.Cells.FocusedCell.Y);
            return (fieldValue != null) ? pivotGridField.GetDisplayText(fieldValue) : null;
        }

        double[] GetValues(Series series) {
            if (series == null || series.Points.Count < 2) {
                return new double[0];
            }
            var values = new double[series.Points.Count];
            for (int i = 0; i < values.Length; i++) {
                values[i] = series.Points[i].Values[0];
            }
            return values;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            PivotGridListEditor.ChartControl.BoundDataChanged += ChartControlOnBoundDataChanged;
            StyleCompareSeries();
            CreateGauge();
            ApplyDataFieldRule();
        }

        void ApplyDataFieldRule() {
            var pivotGridField = PivotGridListEditor.PivotGridControl.Fields[Model.NetIncome.DataFieldName];
            pivotGridField.SummaryType = PivotDataFieldRule.SummaryType;
        }

        void CreateGauge() {
            var modelGauge = Model.NetIncome.Gauge;
            if (modelGauge.GaugeProviderType != null) {
                var layoutControl = (LayoutControl)PivotGridListEditor.Control;
                _pivotGaugeTemplate = (IPivotGaugeTemplate)Activator.CreateInstance(modelGauge.GaugeProviderType, Model.NetIncome);
                var gaugeControl = _pivotGaugeTemplate.GaugeControl;
                var gaugeControlLayoutItem = PivotLayoutControlItem.Parent.Items.FindByName("GaugeControlLayout");
                if (gaugeControlLayoutItem == null && modelGauge.Enabled) {
                    gaugeControl.Name = "Gauge";
                    var layoutControlItem = new LayoutControlItem(layoutControl, gaugeControl) { Name = "GaugeControlLayout" };
                    layoutControlItem.Move(PivotLayoutControlItem, modelGauge.InsertType);
                    layoutControlItem.TextVisible = false;
                    LoadLayoutSettings(layoutControl);
                } else if (gaugeControlLayoutItem != null) {
                    gaugeControlLayoutItem.Visibility = modelGauge.Enabled ? LayoutVisibility.Always : LayoutVisibility.Never;
                }
            }
        }

        private void LoadLayoutSettings(LayoutControl layoutControl) {
            var pivotSettings = ((IModelPivotListView)View.Model).PivotSettings;
            if (!string.IsNullOrEmpty(pivotSettings.LayoutSettings)) {
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(pivotSettings.LayoutSettings));
                layoutControl.RestoreLayoutFromStream(ms);
            }
        }

        void ChartControlOnBoundDataChanged(object sender, EventArgs eventArgs) {
            StyleCompareSeries();
            ApplyDataFieldRule();
        }

        void StyleCompareSeries() {
            Series compareSeries = PivotGridListEditor.ChartControl.Series[_comparePeriodText];
            if (compareSeries != null) {
                var view = compareSeries.View as LineSeriesView;
                if (view != null) {
                    view.LineStyle.DashStyle = DashStyle.Dash;
                    view.LineMarkerOptions.FillStyle.FillMode = FillMode.Solid;
                }
            }
        }

        void PivotGridControlOnCustomDrawCell(object sender, PivotCustomDrawCellEventArgs e) {
            var fieldPeriod = PivotGridListEditor.PivotGridControl.Fields[Model.NetIncome.ColumnFieldName];
            if (Equals(e.GetFieldValue(fieldPeriod), CurrentPeriod)) {
                var fieldMonth = PivotGridListEditor.PivotGridControl.Fields[Model.NetIncome.RowFieldName];
                object month = e.GetFieldValue(fieldMonth);
                var fieldRevenue = PivotGridListEditor.PivotGridControl.Fields[Model.NetIncome.DataFieldName];
                decimal currentValue = Convert.ToDecimal(e.GetCellValue(fieldRevenue)),
                    compareValue = Convert.ToDecimal(e.GetCellValue(new object[] { ComparePeriod },
                        month != null ? new[] { e.GetFieldValue(fieldMonth) } : null, fieldRevenue));
                if (currentValue > compareValue)
                    e.Appearance.ForeColor = CommonColors.GetInformationColor(UserLookAndFeel.Default);
                else if (currentValue < compareValue)
                    e.Appearance.ForeColor = CommonColors.GetCriticalColor(UserLookAndFeel.Default);
            }
        }

        void PivotGridControlOnFieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e) {
            if (e.Field != null && e.Field.FieldName == Model.NetIncome.ColumnFieldName) {
                e.DisplayText = Equals(e.Value, CurrentPeriod) ? _currentPeriodText : _comparePeriodText;
            }
        }

        public new IModelPivotNetIncome Model {
            get { return (IModelPivotNetIncome)base.Model.OptionsPivotGrid; }
        }

        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptionsPivotGrid, IModelPivotNetIncome>();
        }
        #endregion
        #region Implementation of IPivotGroupIntervalEvent
        void IPivotGroupIntervalEvent.Calculate(PivotCustomGroupIntervalEventArgs e) {
            var valueAsDate = (DateTime)e.Value;
            e.GroupValue = valueAsDate >= _currentPeriodDate ? CurrentPeriod : ComparePeriod;
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using System.Linq;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPivotGrid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.SystemModule.ToolTip;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.PivotGrid.Win.Model {


    public interface IModelOptionsColumnPivotGridField : IModelColumnViewColumnOptions {

    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsPivotGridField : IModelColumnOptionsColumnView {
        IModelOptionsColumnPivotGridField OptionsColumnPivotGridField { get; }
    }

    public interface IModelPivotGridSelection : IModelNode {
        Rectangle Rectangle { get; set; }
        [DefaultValue(true)]
        bool Synchronize { get; set; }
        bool RowSelection { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelPivotSettingsEx : IModelPivotSettings {
        bool HideGrid { get; set; }
    }

    public interface IModelPivotGridGeneral : IModelNode {
        LayoutType? Direction { get; set; }
        bool FlipLayout { get; set; }
        [DataSourceProperty("CustomSummaryTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        Type CustomSummaryType { get; set; }
        [Browsable(false)]
        IEnumerable<Type> CustomSummaryTypes { get; }
    }

    [DomainLogic(typeof(IModelPivotGridGeneral))]
    public class ModelPivotGridGeneralDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_CustomSummaryTypes(IModelPivotGridGeneral modelPivotGridGeneral) {
            return FindTypeDescenants(typeof(IPivotCustomSummaryEvent));
        }
    }

    public interface IPivotCustomSummaryEvent : IPivotEvent {
        void Calculate(PivotGridCustomSummaryEventArgs e);
    }

    public interface IPivotGroupIntervalEvent : IPivotEvent {
        void Calculate(PivotCustomGroupIntervalEventArgs e);
    }

    public interface IPivotFieldSortEvent : IPivotEvent {
        void Calculate(PivotGridCustomFieldSortEventArgs e);
    }

    public interface IPivotEvent {
    }

    public class FirstDayOfMonthGroupInterval : IPivotGroupIntervalEvent {
        #region Implementation of ICustomGroupInterval
        public void Calculate(PivotCustomGroupIntervalEventArgs e) {
            var dateTime = ((DateTime)e.Value);
            e.GroupValue = new DateTime(dateTime.Year, dateTime.Month, 1);
        }
        #endregion
    }

    public interface IModelOptionsPivotGrid : IModelOptionsColumnView {
        IModelPivotGridGeneral General { get; }
        IModelPivotRules Rules { get; }
        IModelPivotGridSelection Selection { get; }
    }

    #region IModelPivotFieldSortRule
    [ModelDisplayName("FieldSort")]
    public interface IModelPivotFieldSortRule : IModelPivotFieldRule {
        [DataSourceProperty("CustomSortTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        Type CustomSortType { get; set; }

        [Browsable(false)]
        IEnumerable<Type> CustomSortTypes { get; }
    }

    [DomainLogic(typeof(IModelPivotFieldSortRule))]
    public class PivotFieldSortRuleDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_CustomSortTypes(IModelPivotFieldSortRule modelPivotSortRule) {
            return FindTypeDescenants(typeof(IPivotFieldSortEvent));
        }
    }
    #endregion

    #region IModelPivotFieldToolTipRule
    [ModelDisplayName("FieldToolTip")]
    public interface IModelPivotFieldToolTipRule : IModelPivotFieldRule, IModelToolTipController {

    }


    #endregion



    public abstract class BaseDomainLogic {
        protected static IEnumerable<Type> FindTypeDescenants(Type type) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            return ReflectionHelper.FindTypeDescendants(typeInfo).Where(info => !info.IsAbstract).Select(info => info.Type);
        }
    }
    [DomainLogic(typeof(IModelPivotGroupIntervalRule))]
    public class PivotGroupIntervalRuleDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_CustomGroupIntervalTypes(IModelPivotGroupIntervalRule modelGroupInterval) {
            return FindTypeDescenants(typeof(IPivotGroupIntervalEvent));
        }
    }

    [ModelDisplayName("GroupInterval")]
    public interface IModelPivotGroupIntervalRule : IModelPivotFieldRule {
        [DataSourceProperty("CustomGroupIntervalTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [Required]
        Type GroupIntervalType { get; set; }
        [Browsable(false)]
        IEnumerable<Type> CustomGroupIntervalTypes { get; }

    }
    [ModelAbstractClass]
    public interface IModelPivotFieldRule : IModelPivotRule {
        [Required]
        string FieldName { get; set; }
    }
    public interface IModelPivotRules : IModelNode, IModelList<IModelPivotRule> {
    }

    [ModelDisplayName("SpinEdit")]
    public interface IModelPivotSpinEditRule : IModelPivotSelectionRule {
        IModelRepositoryItemSpinEdit SpinEdit { get; }
    }

    [ModelAbstractClass]
    public interface IModelPivotSelectionRule : IModelPivotRule {
        Point Start { get; set; }
        Point End { get; set; }
    }
    [ModelAbstractClass]
    public interface IModelPivotRule : IModelNodeEnabled {

    }
    [ModelAbstractClass]
    public interface IModelListViewOptionsPivotGrid : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(PivotGridEditorVisibilityCalculator))]
        IModelOptionsPivotGrid OptionsPivotGrid { get; }
    }

    public class PivotGridEditorVisibilityCalculator : EditorTypeVisibilityCalculator<PivotGridListEditor> {
    }

    public interface IModelDrawCellRule : IModelPivotSelectionRule {
        [DataSourceProperty("DrawCellTypes")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [Required]
        Type DrawCellType { get; set; }
        [Browsable(false)]
        IEnumerable<Type> DrawCellTypes { get; }
    }
    [DomainLogic(typeof(IModelDrawCellRule))]
    public class DrawCellRuleDomainLogic : BaseDomainLogic {
        public static IEnumerable<Type> Get_DrawCellTypes(IModelDrawCellRule modelGroupInterval) {
            return FindTypeDescenants(typeof(IPivotDrawCellEvent));
        }
    }
    public interface IPivotDrawCellEvent : IPivotEvent {
        void Calculate(PivotCustomDrawCellEventArgs value);
    }

    public class NegativePositiveDraw : IPivotDrawCellEvent {
        #region Implementation of IPivotDrawCellEvent
        public void Calculate(PivotCustomDrawCellEventArgs e) {
            decimal value = Convert.ToDecimal(e.Value);
            if (value > 0)
                e.Appearance.ForeColor = Color.Green;
            else if (value < 0)
                e.Appearance.ForeColor = Color.Red;
        }
        #endregion
    }
    public interface IModelPivotArea : IModelNode {
        PivotArea PivotArea { get; set; }
    }
    public interface IModelFormatRule : IModelPivotArea, IModelFormatInfo, IModelPivotSelectionRule {
        [RuleValueComparison("IModelFormatRule_PivotArea_not_filter", DefaultContexts.Save, ValueComparisonType.NotEquals, PivotArea.FilterArea)]
        [DefaultValue(PivotArea.ColumnArea)]
        new PivotArea PivotArea { get; set; }
    }
    [ModelAbstractClass]
    public interface IModelFormatInfo : IModelNode {
        string FormatString { get; set; }
        FormatType FormatType { get; set; }
    }
}

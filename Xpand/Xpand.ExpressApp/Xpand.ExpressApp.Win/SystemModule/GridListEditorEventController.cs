using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;
using Fasterflect;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule {

    public class GridListEditorEventController:ViewController<ListView>,IModelExtender{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var columnsListEditor = View.Editor as ColumnsListEditor;
            if (columnsListEditor != null){
                var gridView = columnsListEditor.GridView();
                if (gridView != null){
                    gridView.PopupMenuShowing+=GridViewOnPopupMenuShowing;
                    gridView.CustomSummaryCalculate+=GridViewOnCustomSummaryCalculate;
                }
            }
        }

        private void GridViewOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e){
            if (e.MenuType != GridMenuType.Summary) return;
            var ruleCollector = new RuleCollector(View.Model);
            foreach (var modelGridViewRule in ruleCollector.GetCustomSummeryCalculateRules(e.HitInfo.Column.PropertyName())) {
                var footerMenu = ((GridViewFooterMenu) e.Menu);
                var menuItem = NewDXMenuItem(sender, e, modelGridViewRule);
                foreach (DXMenuItem item in footerMenu.Items)
                    item.Enabled = true;
                footerMenu.Items.Add(menuItem);    
            }            
        }

        private static DXMenuItem NewDXMenuItem(object sender, PopupMenuShowingEventArgs e,IModelGridViewRuleCustomSummaryCalculate modelGridViewRule){
            bool check = e.HitInfo.Column.SummaryItem.SummaryType == SummaryItemType.Custom &&Equals(modelGridViewRule, e.HitInfo.Column.SummaryItem.Tag);
            DXMenuItem menuItem = new DXMenuCheckItem(modelGridViewRule.Caption, check, null, (o, args) =>{
                var item = ((DXMenuItem) sender);
                var col = ((GridColumn) item.Tag);
                col.SummaryItem.Tag = modelGridViewRule;
                col.SummaryItem.SetSummary(SummaryItemType.Custom, string.Empty);
            });
            menuItem.Tag = e.HitInfo.Column;
            return menuItem;
        }

        void GridViewOnCustomSummaryCalculate(object sender, CustomSummaryEventArgs e){
            var item = ((GridColumnSummaryItem)e.Item);
            var rule = item.Tag as IModelGridViewRuleCustomSummaryCalculate;
            if (rule != null){
                var ruleCollector = new RuleCollector(View.Model);
                var customSummaryCalculateEvent = ruleCollector.CreateInstance<ICustomSummaryCalculateEvent>(Frame, rule.Controller);
                if (e.SummaryProcess == CustomSummaryProcess.Calculate &&!CriteriaOperator.Parse(rule.Criteria).Fit(((GridView) sender).GetRow(e.RowHandle))) 
                    return;
                customSummaryCalculateEvent.Calculate(e);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptionsGridView, IModelOptionsGridViewRules>();
        }
    }

    public class CustomSummaryCalculateController:ViewController<ListView>,ICustomSummaryCalculateEvent{
        private int _validRowCount;

        public void Calculate(CustomSummaryEventArgs e){
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                _validRowCount = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Calculate) {
                _validRowCount++;
            }
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                e.TotalValue = _validRowCount;

        }
    }
    [ModelAbstractClass]
    public interface IModelGridViewRule:IModelNodeEnabled{
        [ModelValueCalculator("Id")]
        [Required]
        string Caption { get; set; }

        [TypeConverter(typeof(StringToTypeConverterBase))]
        [DataSourceProperty("Controllers")]
        [Required]
        Type Controller { get; set; }

        [Browsable(false)]
        IEnumerable<Type> Controllers { get; }
    }

    public interface IModelGridViewRuleCustomSummaryCalculate:IModelGridViewRule{
        [DefaultValue("*")]
        string Members { get; set; }
        string Criteria { get; set; }
        [Browsable(false)]
        ITypeInfo TypeInfo { get; }
    }

    [DomainLogic(typeof(IModelGridViewRuleCustomSummaryCalculate))]
    public class ModelGridViewRuleCustomSummaryCalculateDomainLogic{

        public static IEnumerable<Type> Get_Controllers(IModelGridViewRuleCustomSummaryCalculate calculate) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(ICustomSummaryCalculateEvent)).Implementors.Where(info => !info.IsInterface).Select(info => info.Type);
        }
    }

    public interface IModelGridViewRules:IModelList<IModelGridViewRule>,IModelNode{
         
    }

    public interface IModelOptionsGridViewRules {
        IModelGridViewRules Rules { get; }
    }

    class RuleCollector {
        readonly IModelGridViewRules _modelOptionsGridViewRules;

        public RuleCollector(IModelListView modelListView){
            _modelOptionsGridViewRules= ((IModelOptionsGridViewRules) ((IModelListViewOptionsGridView) modelListView).GridViewOptions).Rules;
        }

        public IEnumerable<IModelGridViewRuleCustomSummaryCalculate> GetCustomSummeryCalculateRules(params string[] members){
            return GetRules<IModelGridViewRuleCustomSummaryCalculate>().Where(calculate =>{
                if (calculate.Members != "*"){
                    var strings = calculate.Members.Split(';');
                    return members.Any(s => strings.Any(s1 => s == s1));
                }
                return true;
            });
        }

        IEnumerable<TRule> GetRules<TRule>() where TRule : IModelGridViewRule {
            return _modelOptionsGridViewRules.Where(rule => rule.NodeEnabled).OfType<TRule>();
        }

        public IEnumerable<T> ControlEvents<T, TV>(Frame frame, IEnumerable<IModelGridViewRule> modelPivotFieldRules, Func<TV, Type> action)
            where T : IControlEvent
            where TV : IModelGridViewRule {
            return modelPivotFieldRules.Select(intervalRule => action.Invoke((TV)intervalRule)).Select(type => CreateInstance<T>(frame, type));
        }

        public T CreateInstance<T>(Frame frame, Type type) where T : IControlEvent {
            return typeof(Controller).IsAssignableFrom(type) ? frame.Controllers.Values.OfType<T>().First() : (T)type.CreateInstance();
        }
    }

    public interface ICustomSummaryCalculateEvent : IControlEvent {
        void Calculate(CustomSummaryEventArgs e);
    }

    public interface IControlEvent{
    }
}


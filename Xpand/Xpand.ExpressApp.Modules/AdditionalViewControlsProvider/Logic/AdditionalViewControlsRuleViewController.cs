using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController : ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        Dictionary<string, object> _infoToLayoutMapCore;
        protected Dictionary<string, object> RuleToLayoutMap {
            get { return _infoToLayoutMapCore ?? (_infoToLayoutMapCore = new Dictionary<string, object>()); }
        }
        protected void ResetInfoToLayoutMap() {
            RuleToLayoutMap.Clear();
        }
        protected void FillInfoToLayoutMap(ViewItem detailViewItem, IModelViewLayoutElement itemModel, object layoutItem) {
            if (detailViewItem is AdditionalViewControlsItem) {
                var id = ((AdditionalViewControlsItem)detailViewItem).Model.Rule.Id;
                if (RuleToLayoutMap.ContainsKey(id))
                    RuleToLayoutMap[id] = layoutItem;
                else
                    RuleToLayoutMap.Add(id, layoutItem);
            }

        }

        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> info, ExecutionContext executionContext) {
            if (Frame != null) {
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = GetContainerControl(viewSiteTemplate, info.Rule);
                if (viewSiteControl != null) {
                    IAdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, info.View.ObjectTypeInfo.Type);
                    Type controlType = calculator.ControlsRule.ControlType;
                    ICollection controls = GetControls(viewSiteControl);
                    IAdditionalViewControl additionalViewControl = FindControl(info.Rule, controls);
                    if (info.Active && ViewContextIsCorrect(info.Rule)) {

                        var control = GetControl(controlType, additionalViewControl, info);
                        control.Rule = info.Rule;
                        ReflectionHelper.CreateObject(calculator.ControlsRule.DecoratorType, new[] { info.View, (object)control, info.Rule });
                        if (additionalViewControl == null) {
                            AddControl(control, controls, info);
                            InitializeControl(control, info, calculator, executionContext);
                        }
                    } else if (additionalViewControl != null) {
                        controls.GetType().GetMethod("Remove").Invoke(controls, new[] { additionalViewControl });
                    }
                }
            }

        }

        bool ViewContextIsCorrect(IAdditionalViewControlsRule rule) {
            return rule.Position != Position.DetailViewItem || !(View is ListView);
        }

        protected virtual IAdditionalViewControl GetControl(Type controlType, IAdditionalViewControl additionalViewControl, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            var control = additionalViewControl ?? Activator.CreateInstance(controlType) as IAdditionalViewControl;
            if (control is ISupportLayoutManager) {
                if (info.Rule.Position != Position.DetailViewItem)
                    throw new ArgumentException("Rule with Id:" + info.Rule.Id + " position should be set to " + Position.DetailViewItem);
                if (RuleToLayoutMap.ContainsKey(info.Rule.Id))
                    ((ISupportLayoutManager)control).LayoutItem = RuleToLayoutMap[info.Rule.Id];
            }
            return control;
        }

        protected object GetContainerControl(IViewSiteTemplate viewSiteTemplate, IAdditionalViewControlsRule rule) {
            if (rule.Position == Position.DetailViewItem && View is DetailView) {
                var modelAdditionalViewControlsItem = ((DetailView)View).Items.OfType<AdditionalViewControlsItem>().Where(item => item.Model.Rule.Id == rule.Id).FirstOrDefault();
                return modelAdditionalViewControlsItem != null ? modelAdditionalViewControlsItem.Control : null;
            }
            return viewSiteTemplate.ViewSiteControl;
        }

        ICollection GetControls(object viewSiteControl) {
            return (ICollection)(viewSiteControl.GetType().GetProperty("Controls").GetValue(viewSiteControl, null));
        }

        IAdditionalViewControl FindControl(IAdditionalViewControlsRule rule, ICollection controls) {
            return controls.OfType<IAdditionalViewControl>().Where(o => o.Rule.Id == rule.Id).FirstOrDefault();
        }

        protected virtual void RemoveControl(IList controls, object firstOrDefault, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            if (info.Rule.Position != Position.DetailViewItem)
                controls.Remove(firstOrDefault);
        }

        protected virtual void AddControl(object control, object controls, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            controls.GetType().GetMethod("Add").Invoke(controls, new[] { control });
        }


        protected override IModelLogic GetModelLogic() {
            return ((IModelApplicationAdditionalViewControls)Application.Model).AdditionalViewControls;
        }





        protected virtual void InitializeControl(object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context) {
            if (control is ISupportAppeareance) {
                var supportAppeareance = ((ISupportAppeareance)control);
                supportAppeareance.BackColor = additionalViewControlsRule.Rule.BackColor;
                supportAppeareance.Height = additionalViewControlsRule.Rule.Height;
                supportAppeareance.ForeColor = additionalViewControlsRule.Rule.ForeColor;
                supportAppeareance.FontStyle = additionalViewControlsRule.Rule.FontStyle;
                supportAppeareance.FontSize = additionalViewControlsRule.Rule.FontSize;
            }
        }
    }
}
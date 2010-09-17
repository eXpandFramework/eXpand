using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;
using System.Linq;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController :ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        
        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> info, ExecutionContext executionContext) {
            if (Frame != null){
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = GetContainerControl(viewSiteTemplate,info.Rule);
                if (viewSiteControl != null){
                    IAdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, info.View.ObjectTypeInfo.Type);
                    Type controlType = calculator.ControlsRule.ControlType;
                    IList controls = GetControls(viewSiteControl);
                    if (info.Active) {
                        object o = FindControl(info, controlType, controls);
                        object control = GetControl(controlType, o,info);
                        ReflectionHelper.CreateObject(calculator.ControlsRule.DecoratorType, new[] { info.View, control, info.Rule });
                        if (info.Rule.NotUseSameType||o== null) {
                            AddControl(control, controls);
                            InitializeControl( control, info, calculator, executionContext);
                        }
                        ((AdditionalViewControlsRule) info.Rule).Control = control;
                    }
                    else {
                        object control = ((AdditionalViewControlsRule) info.Rule).Control;
                        controls.Remove(control);
                    }
                }
            }

        }

        protected virtual object GetControl(Type controlType, object o, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            return o??Activator.CreateInstance(controlType);
        }

        object GetContainerControl(IViewSiteTemplate viewSiteTemplate, IAdditionalViewControlsRule rule) {
            if (rule.Position==Position.DetailViewItem&&View is XpandDetailView) {
                var modelAdditionalViewControlsItem = ((XpandDetailView)View).Items.OfType<AdditionalViewControlsItem>().Where(item => item.Model.Rule.Id == rule.Id).FirstOrDefault();
                return modelAdditionalViewControlsItem != null ? modelAdditionalViewControlsItem.Control : null;
            }
            return viewSiteTemplate.ViewSiteControl;
        }

        IList GetControls(object viewSiteControl) {
            return ((IList)viewSiteControl.GetType().GetProperty("Controls").GetValue(viewSiteControl, null));
        }

        object FindControl(LogicRuleInfo<IAdditionalViewControlsRule> info, Type controlType, IList controls) {
            if (info.Rule.NotUseSameType&&info.Active)
                return null;
            object firstOrDefault = controls.OfType<object>().Where(o => controlType==o.GetType()).FirstOrDefault();
//            RemoveControl(controls, firstOrDefault, info);
            return firstOrDefault;
        }

        protected virtual void RemoveControl(IList controls, object firstOrDefault, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            controls.Remove(firstOrDefault);
        }

        protected virtual void AddControl(object control, IEnumerable controls) {
            controls.GetType().GetMethod("Add").Invoke(controls, new[] { control });
        }

        


        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup){
            return ((IModelApplicationAdditionalViewControls)Application.Model).AdditionalViewControls.GroupContexts;
        }




        protected void InitializeControl(object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context) {
            if (control is ISupportAppeareance)
                ((ISupportAppeareance) control).BackColor=additionalViewControlsRule.Rule.BackColor;
        }
    }
}
using System;
using System.Collections;
using System.Linq;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public abstract class AdditionalViewControlsRuleViewController :ConditionalLogicRuleViewController<IAdditionalViewControlsRule> {
        
        public override void ExecuteRule(LogicRuleInfo<IAdditionalViewControlsRule> info, ExecutionContext executionContext) {
            if (Frame != null){
                var viewSiteTemplate = Frame.Template as IViewSiteTemplate;
                if (viewSiteTemplate == null)
                    return;
                object viewSiteControl = viewSiteTemplate.ViewSiteControl;
                if (viewSiteControl != null){
                    IAdditionalViewControlsRule additionalViewControlsRule = info.Rule;
                    var calculator = new AdditionalViewControlsProviderCalculator(additionalViewControlsRule, info.View.ObjectTypeInfo.Type);
                    Type controlType = calculator.ControlsRule.ControlType;
                    if (info.Active) {
                        object control = Activator.CreateInstance(controlType);
                        AddControl(viewSiteControl, control, info, calculator, executionContext);
                    }
                    else {
                        RemoveControl(viewSiteControl, controlType);
                    }
                }
            }

        }
        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup){
            return ((IModelApplicationAdditionalViewControls)Application.Model).AdditionalViewControls.GroupContexts;
        }

        protected abstract void RemoveControl(object viewSiteControl, Type controlType);


        protected object GetControl(IEnumerable collection, object control,
                                    AdditionalViewControlsProviderCalculator calculator,
                                    LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule){
            object o = additionalViewControlsRule.Rule.UseSameIfFound
                           ? (collection.OfType<object>().Where(control1 => control1.GetType().Equals(control.GetType())).
                                  FirstOrDefault() ?? control): control;
            Activator.CreateInstance(calculator.ControlsRule.DecoratorType, new[] { additionalViewControlsRule.View, o, additionalViewControlsRule.Rule });
            return o;
        }


        protected abstract object AddControl(object viewSiteControl, object control, LogicRuleInfo<IAdditionalViewControlsRule> additionalViewControlsRule, AdditionalViewControlsProviderCalculator calculator, ExecutionContext context);
    }
}